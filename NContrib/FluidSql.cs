﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NContrib.Extensions;

namespace NContrib {

    public struct FluidSqlEventHandler<T> {

        public Action<FluidSql, T> Handler { get; private set; }

        public FluidSqlEventHandler(Action<FluidSql, T> handler) : this() {
            Handler = handler;
        }
    }

    public class FluidSql {

        public static class BuiltinHandlers {
            
            public static string FormatProcedureError(FluidSql fs, SqlException ex) {

                var args = fs.Parameters.Select(p => "@" + p.Key + " = " + p.Value).Join(", ");

                return "Error executing procedure " + fs.Command.CommandText + " (" + args + "): " + ex.Message;
            }
        }

        public SqlConnection Connection { get; protected set; }

        public SqlCommand Command { get; protected set; }

        /// <summary>
        /// <see cref="Connection"/> is automatically closed after a command excution
        /// </summary>
        public bool AutoClose { get; protected set; }

        public int CommandExecutionCount { get; protected set; }

        public int RecordsAffected { get; protected set; }

        protected readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();

        protected SqlParameter ReturnValueParameter { get; set; }

        protected List<FluidSqlEventHandler<SqlException>> ErrorHandlers = new List<FluidSqlEventHandler<SqlException>>();

        protected List<FluidSqlEventHandler<SqlInfoMessageEventArgs>> InfoHandlers = new List<FluidSqlEventHandler<SqlInfoMessageEventArgs>>();

        public FluidSql(string connectionString, bool autoClose = true)
            : this(new SqlConnection(connectionString)) {

            AutoClose = autoClose;
        }

        public FluidSql(SqlConnection connection) {
            Connection = connection;
        }

        public FluidSql Error(Action<FluidSql, SqlException> handler) {
            ErrorHandlers.Add(new FluidSqlEventHandler<SqlException>(handler));
            return this;
        }

        public FluidSql Info(Action<FluidSql, SqlInfoMessageEventArgs> handler) {
            InfoHandlers.Add(new FluidSqlEventHandler<SqlInfoMessageEventArgs>(handler));
            return this;
        }

        public FluidSql AddParameter(string name, object value) {
            Parameters.Add(name, value);
            return this;
        }

        public FluidSql AddParameters(object parameters) {
            if (parameters == null)
                return this;

            return AddParameters(parameters.GetType().GetProperties()
                                     .ToDictionary(p => p.Name, p => p.GetValue(parameters, null)));
        }

        public FluidSql AddParameters(IDictionary<string, object> parameters) {
            parameters.ToList().ForEach(p => AddParameter(p.Key, p.Value));
            return this;
        }

        public T GetReturnValue<T>() {
            if (ReturnValueParameter == null)
                throw new Exception("No return value parameter has been initialized");

            return ReturnValueParameter.Value.ConvertTo<T>();
        }

        #region Public setup
        public FluidSql CreateProcedureCommand(string procedureName, object parameters = null) {
            CreateCommand(procedureName, CommandType.StoredProcedure, parameters);
            return this;
        }

        public FluidSql CreateTextCommand(string textCommand, object parameters = null) {
            CreateCommand(textCommand, CommandType.Text, parameters);
            return this;
        }
        #endregion

        #region Public execution
        public int ExecuteNonQuery() {
            return InternalExecuteNonQuery();
        }

        public T ExecuteReturnValue<T>() {
            InternalExecuteNonQuery();
            return GetReturnValue<T>();
        }

        public T ExecuteScalar<T>() {
            return InternalExecuteScalar().ConvertTo<T>();
        }

        public T ExecuteScalar<T>(string commandText, object parameters = null) {
            CreateCommand(commandText, CommandType.Text, parameters);
            return InternalExecuteScalar().ConvertTo<T>();
        }

        public IEnumerable<IDictionary<string, object>> ExecuteDictionaries() {
            return ExecuteDictionaries<object>();
        }

        public IEnumerable<IDictionary<string, TValue>> ExecuteDictionaries<TValue>() {
            var temp = ExecuteAndTransform(dr => dr.GetRowAsDictionary<TValue>());
            OnDataRead();
            return temp;
        }

        public Dictionary<TKey, TValue> ExecuteVerticalDictionary<TKey, TValue>(int keyCol = 0, int valCol = 1) {
            var temp = new Dictionary<TKey, TValue>();
            using (var dr = InternalExecuteReader()) {
                while (dr.Read())
                    temp.Add(dr.GetValue<TKey>(keyCol), dr.GetValue<TValue>(valCol));
            }
            OnDataRead();
            return temp;
        }

        public Dictionary<TKey, TValue> ExecuteVerticalDictionary<TKey, TValue>(string keyCol, string valCol) {
            var temp = new Dictionary<TKey, TValue>();
            using (var dr = InternalExecuteReader()) {
                while (dr.Read())
                    temp.Add(dr.GetValue<TKey>(keyCol), dr.GetValue<TValue>(valCol));
            }
            OnDataRead();
            return temp;
        }

        public IEnumerable<T> ExecuteAndTransform<T>(Converter<IDataReader, T> converter) {
            IEnumerable<T> temp;
            using (var dr = InternalExecuteReader()) {
                temp = dr.Transform(converter);
            }
            OnDataRead();
            return temp;
        }
        #endregion

        #region Inline value assignment
        /*
         * These need to be re-worked as post-execution events so they can go in-line before the execution
         * 
        /// <summary>
        /// Gets the stored return value and assigns it somewhere
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assigner"></param>
        /// <returns></returns>
        public FluidSql AssignReturnValue<T>(Action<T> assigner) {
            assigner(GetReturnValue<T>());
            return this;
        }

        public FluidSql AssignRecordsAffected(Action<int> assigner) {
            assigner(RecordsAffected);
            return this;
        }

        public FluidSql AssignCommandExecutionCount(Action<int> assigner) {
            assigner(CommandExecutionCount);
            return this;
        }
        */
        #endregion

        #region Internal Setup
        protected void PrepareCommand() {
            if (Parameters.Count > 0)
                Parameters.ToList().ForEach(p => Command.Parameters.AddWithValue(p.Key, p.Value));
        }

        protected void CreateCommand(string commandText, CommandType commandType, object parameters = null) {
            Command = new SqlCommand(commandText, Connection);
            Command.CommandType = commandType;
            AddParameters(parameters);

            if (commandType == CommandType.StoredProcedure)
                AddReturnValueParameter();
        }

        protected void AddReturnValueParameter() {
            if (Command == null)
                throw new Exception("Command has not been initialized");

            if (Command.CommandType != CommandType.StoredProcedure)
                throw new Exception("Return values are only supported on procedures");

            ReturnValueParameter = Command.Parameters.Add("@RETURN_VALUE", SqlDbType.Variant);
            ReturnValueParameter.Direction = ParameterDirection.ReturnValue;
        }
        #endregion

        #region Internal execution
        protected int InternalExecuteNonQuery() {
            return (RecordsAffected = InternalExecute(Command.ExecuteNonQuery, true));
        }

        protected SqlDataReader InternalExecuteReader() {
            return InternalExecute(Command.ExecuteReader);
        }

        protected object InternalExecuteScalar() {
            return InternalExecute(Command.ExecuteScalar, true);
        }

        protected T InternalExecute<T>(Func<T> executor, bool dataReadComplete = false) {
            OnExecutingCommand();

            try {
                return executor();
            }
            catch (SqlException ex) {
                if (ErrorHandlers.Count == 0)
                    throw ex;

                foreach (var h in ErrorHandlers)
                    h.Handler(this, ex);
            }
            finally {
                OnExecutedCommand(dataReadComplete);
            }

            return default(T);
        }
        #endregion

        #region Internal Events
        protected void OnExecutingCommand() {
            if (Connection.State != ConnectionState.Open) {
                try {
                    Connection.Open();
                }
                catch (SqlException ex) {
                    if (ErrorHandlers.Count == 0)
                        throw ex;

                    foreach (var h in ErrorHandlers)
                        h.Handler(this, ex);
                }
            }

            if (InfoHandlers.Count > 0)
                InfoHandlers.ToList()
                    .ForEach(h => Connection.InfoMessage += (sender, e) => h.Handler(this, e));

            PrepareCommand();
        }

        protected void OnExecutedCommand(bool dataReadComplete = false) {
            CommandExecutionCount++;

            if (dataReadComplete)
                OnDataRead();
        }

        protected void OnDataRead() {
            if (AutoClose && Connection.State != ConnectionState.Closed)
                Connection.Close();
        }

        protected void OnConnectionError() {
            
        }

        protected void OnCommandError() {
            
        }
        #endregion
    }
}