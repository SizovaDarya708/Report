using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraInteraction.Dtos;

public class JiraUserDto
{
    public string GetDepartment()
    {
        return groups?.items
            .Select(x => x.name.ToLower())
            .Where(x => x.Contains("отдел"))
            .FirstOrDefault() ?? string.Empty;
    }
    public Groups? groups { get; set; }
}

public class  Groups
{
    public List<Item>? items { get; set; }
    
}

public class Item { 
    public string name { get; set; }
}
