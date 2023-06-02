namespace Sonarqube.Analyzer.Request
{
    public class AnalyzerRequest
    {
        public string Key { get; set; }
        public Bugs Bugs { get; set; }
        public CodeSmell CodeSmell { get; set; }
        public Vulnerability Vulnerability { get; set; }
    }
}
