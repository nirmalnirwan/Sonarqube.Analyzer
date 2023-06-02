using System;
using System.Collections.Generic;

namespace Sonarqube.Analyzer.Response.Analyzer
{
    public class ResponseAnalyzer
    {
        public int total { get; set; }
        public int p { get; set; }
        public int ps { get; set; }
        public Paging paging { get; set; }
        public int effortTotal { get; set; }
        public List<Issue> issues { get; set; }
        public List<Component> components { get; set; }
        public List<object> facets { get; set; }
    }

    public class Component
    {
        public string key { get; set; }
        public bool enabled { get; set; }
        public string qualifier { get; set; }
        public string name { get; set; }
        public string longName { get; set; }
        public string path { get; set; }
    }

    public class Flow
    {
        public List<Location> locations { get; set; }
    }

    public class Issue
    {
        public string key { get; set; }
        public string rule { get; set; }
        public string severity { get; set; }
        public string component { get; set; }
        public string project { get; set; }
        public int line { get; set; }
        public string hash { get; set; }
        public TextRange textRange { get; set; }
        public List<Flow> flows { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string effort { get; set; }
        public string debt { get; set; }
        public string assignee { get; set; }
        public string author { get; set; }
        public List<string> tags { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime updateDate { get; set; }
        public string type { get; set; }
        public string scope { get; set; }
        public bool quickFixAvailable { get; set; }
        public List<object> messageFormattings { get; set; }
    }

    public class Location
    {
        public string component { get; set; }
        public TextRange textRange { get; set; }
        public List<object> msgFormattings { get; set; }
    }

    public class Paging
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
    }

    public class TextRange
    {
        public int startLine { get; set; }
        public int endLine { get; set; }
        public int startOffset { get; set; }
        public int endOffset { get; set; }
    }


}
