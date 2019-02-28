using System;
using System.Collections.Generic;

namespace NT{

    [Serializable]
    public class DummyConnection
    {
        public string hi = "hello";

        public List<string> values = new List<string>(){"a", "b"};

        public string[] valuesArray = {"c", "d"};

    }

}