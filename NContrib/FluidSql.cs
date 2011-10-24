using System;
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

    public class CommandExecutedEventArgs : EventArgs {

        public TimeSpan TimeTaken { get; protected set; }

        public string Command { get; protected set; }

        public SqlParameterCollection Parameters { get; protected set; }

        public CommandExecutedEventArgs(TimeSpan timeTaken, string command, SqlParameterCollection parameters) {
            TimeTaken = timeTaken;
            Command = command;
            Parameters = parameters;
        }
    }

    public class FluidSql {

        public static class BuiltinHandlers {
            
            public static string FormatProcedureError(FluidSql fs, SqlException ex) {

                var args = fs.Parameters.Select(p => "@" + p.Key + " = " + p.Value).Join(", ");

                return "Error executing procedure " + fs.Command.CommandText + " (" + args + "): " + ex.Message;
            }
        }

        public static class FieldNameConverters {
            
            public static string TitleCase(string name) {
                return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name).Replace("_", "");
            }

            public static string CamelCase(string name) {
                return name.ToCamelCase();
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

        public TimeSpan TimeTaken { get; protected set; }

        public event EventHandler<CommandExecutedEventArgs> Executed;

        protected readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();

        protected SqlParameter ReturnValueParameter { get; set; }

        protected List<FluidSqlEventHandler<SqlException>> ErrorHandlers = new List<FluidSqlEventHandler<SqlException>>();

        protected List<FluidSqlEventHandler<SqlInfoMessageEventArgs>> InfoHandlers = new List<FluidSqlEventHandler<SqlInfoMessageEventArgs>>();

        protected List<FluidSqlEventHandler<StateChangeEventArgs>> ConnectionStateChangeHandlers = new List<FluidSqlEventHandler<StateChangeEventArgs>>();

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

        public FluidSql ConnectionStateChange(Action<FluidSql, StateChangeEventArgs> handler) {
            ConnectionStateChangeHandlers.Add(new FluidSqlEventHandler<StateChangeEventArgs>(handler));
            return this;
        }

        public FluidSql ExecutedHandler(EventHandler<CommandExecutedEventArgs> handler) {
            Executed += handler;
            return this;
        }

        public FluidSql AddParameter(string name, object value) {
            Parameters.Add(name.ToSnakeCase(), value);
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

        public FluidSql CreateInsertCommand(string table, IDictionary<string, object> fields) {

            var sql = "insert into " + table +
                " (" + fields.Keys.Join(", ") + ")" +
                " values(" + fields.Keys.Select(k => "@" + k).Join(", ") + ")";

            CreateTextCommand(sql);
            AddParameters(fields);

            return this;
        }

        public FluidSql CreateUpdateCommand(string table, IDictionary<string, object> fields, string where) {

            var sql = "update " + table + " set " + fields.Keys.Select(f => f + " = @" + f).Join(", ") + " where " + where;

            CreateTextCommand(sql);
            AddParameters(fields);

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

        public IEnumerable<IDictionary<string, object>> ExecuteDictionaries(Func<string, string> fieldNameConverter = null) {
            return ExecuteDictionaries<object>(fieldNameConverter);
        }

        public IEnumerable<IDictionary<string, TValue>> ExecuteDictionaries<TValue>(Func<string, string> fieldNameConverter = null) {
            var temp = ExecuteAndTransform(dr => dr.GetRowAsDictionary<TValue>(fieldNameConverter));
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

        public ILookup<TKey, TValue> ExecuteVerticalLookup<TKey, TValue>(int keyCol = 0, int valCol = 0) {

            return ExecuteAndTransform(r => new {Key = r.GetValue<TKey>(keyCol), Value = r.GetValue<TValue>(valCol)})
                .ToLookup(o => o.Key, o => o.Value);
        }

        public ILookup<TKey, TValue> ExecuteVerticalLookup<TKey, TValue>(string keyCol, string valCol) {

            return ExecuteAndTransform(r => new { Key = r.GetValue<TKey>(keyCol), Value = r.GetValue<TValue>(valCol) })
                .ToLookup(o => o.Key, o => o.Value);
        }

        public T[] ExecuteArray<T>() {
            return ExecuteAndTransform(r => r.GetValue<T>(0)).ToArray();
        }

        public T[] ExecuteArray<T>(int keyCol) {
            return ExecuteAndTransform(r => r.GetValue<T>(keyCol)).ToArray();
        }

        public T[] ExecuteArray<T>(string keyCol) {
            return ExecuteAndTransform(r => r.GetValue<T>(keyCol)).ToArray();
        }

        public IEnumerable<T> ExecuteAndTransform<T>(Converter<IDataReader, T> converter) {
            IEnumerable<T> temp;
            using (var dr = InternalExecuteReader()) {
                temp = dr.TransformAll(converter);
            }
            OnDataRead();
            return temp;
        }

        public T ExecuteScopeIdentity<T>() {
            Command.CommandText += "; select scope_identity()";
            return ExecuteScalar<T>();
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
            Command = new SqlCommand(commandText, Connection) {CommandType = commandType};
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
                var start = DateTime.Now;
                var result = executor();
                TimeTaken = DateTime.Now - start;

                return result;
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

            if (ConnectionStateChangeHandlers.Count > 0)
                ConnectionStateChangeHandlers.ToList()
                    .ForEach(h => Connection.StateChange += (sender, e) => h.Handler(this, e));

            if (InfoHandlers.Count > 0)
                InfoHandlers.ToList()
                    .ForEach(h => Connection.InfoMessage += (sender, e) => h.Handler(this, e));

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

            PrepareCommand();
        }

        protected void OnExecutedCommand(bool dataReadComplete = false) {
            CommandExecutionCount++;

            if (Executed != null)
                Executed(this, new CommandExecutedEventArgs(TimeTaken, Command.CommandText, Command.Parameters));

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
