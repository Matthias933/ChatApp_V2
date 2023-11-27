namespace BuilderExample
{
    public class Program
    {
        static void Main(string[] args)
        {
            Director director = new Director();
            Builder builder = new Builder();

            director.Builder = builder;
            Console.WriteLine("Single Type Map");
            director.BuildSingleMaterial();
            Console.WriteLine(builder.GetMap().ShowMap());

            Console.WriteLine("Multiple Type Map");
            director.BuildMultipleMaterial();
            Console.WriteLine(builder.GetMap().ShowMap());

            builder.BuildTree();
            builder.BuildGrass();
            Console.WriteLine(builder.GetMap().ShowMap());
        }
    }


    public interface IBuilder
    {
        void BuildGrass();

        void BuildStone();

        void BuildTree();
    }

    public class Builder : IBuilder
    {
        private Map map = new Map();
        public void BuildGrass()
        {
            map.Add("Grass");
        }

        public void BuildStone()
        {
            map.Add("Stone");
        }

        public void BuildTree()
        {
            map.Add("Tree");
        }

        public Builder()
        {
            //Stellt sicher das er eine Neue Map hat
            Reset();
        }

        public void Reset()
        {
            map = new Map();
        }

        public Map GetMap()
        {
            //Speichert sich die Aktuelle Map
            Map result = map;

            Reset();

            //Gibt die gebaute Map zurück
            return result;
        }
    }

    public class Map
    {   
        //Liste wo die Ganzen map Teile gespeichert werden
        private List<object> mapParts = new List<object>();

        //Fügt ein neues Teil zu der Map hinzu
        public void Add(string mapPart)
        { 
            mapParts.Add(mapPart);
        }

        //Gibt die Zussamengebaute map in einem string zurück
        public string ShowMap()
        {
            string map = string.Empty;

            for (int i = 0; i < mapParts.Count; i++)
            {
                map += mapParts[i] + ", ";
            }

            map = map.Remove(map.Length - 2);

            return "Builded Map: " + map;
        }
    }

    public class Director
    {
        private IBuilder builder;

        public IBuilder Builder
        {
            set { builder = value; }
        }

        //Kan vorgefertite Map enthalten
        public void BuildSingleMaterial()
        {
            builder.BuildStone();
        }

        public void BuildMultipleMaterial()
        {
            builder.BuildStone();
            builder.BuildTree();
            builder.BuildGrass();
        }
    }


}