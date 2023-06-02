using System;
using System.Collections.Generic;

namespace Sonarqube.Analyzer.Response.ProjectsInfo
{
    public class ProjectsInfo
    {
        public Paging paging { get; set; }
        public List<Component> components { get; set; }
    }
    public class Component
    {
        public string key { get; set; }
        public string name { get; set; }
        public string qualifier { get; set; }
        public string visibility { get; set; }
        public DateTime lastAnalysisDate { get; set; }
        public string revision { get; set; }
    }

    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
    }
}
