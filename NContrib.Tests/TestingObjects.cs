namespace NContrib.Tests {

    public struct Point {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
            : this() {
            X = x;
            Y = y;
        }
    }

    public enum Operators {
        None,
        Tre,
        Telia,
        Telenor,
        Tele2
    }

}
