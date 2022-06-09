using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace RubyOnBrain.RecommenderSystem
{
    public class RecommenderSystem
    {
        public RecommenderSystem()
        { 
            
        }

        public void StartScript()
        {
            ScriptEngine engine = Python.CreateEngine();

            engine.Execute("rec_sys.py");
        }
    }
}