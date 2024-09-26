using System.Text.RegularExpressions;

namespace SaySoundsSharp;

public class ConfigParser
{
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> configData;

    public ConfigParser(string filePath)
    {
        configData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        ParseConfigFile(filePath);
    }


    private void ParseConfigFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        string mainSection = null;
        string subSection = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
            {
                continue; // Skip empty lines and comments
            }

            if (trimmedLine.StartsWith("\"") && trimmedLine.EndsWith("\""))
            {
                if (Regex.IsMatch(trimmedLine, "^\".*\"\\s+\".*\"$"))
                {
                    // This is a key-value pair
                    var match = Regex.Match(trimmedLine, "^\"(.*)\"\\s+\"(.*)\"$");
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value;

                        if (mainSection != null && subSection != null)
                        {
                            if (!configData.ContainsKey(mainSection))
                            {
                                configData[mainSection] = new Dictionary<string, Dictionary<string, string>>();
                            }

                            if (!configData[mainSection].ContainsKey(subSection))
                            {
                                configData[mainSection][subSection] = new Dictionary<string, string>();
                            }

                            configData[mainSection][subSection][key] = value;
                        }
                    }
                }
                else
                {
                    // This is a section header
                    string sectionName = trimmedLine.Trim('"');

                    if (mainSection == null)
                    {
                        mainSection = sectionName;
                    }
                    else if (subSection == null)
                    {
                        subSection = sectionName;
                    }
                }
            }
            else if (trimmedLine == "{")
            {
                // Start of a new section, do nothing here
            }
            else if (trimmedLine == "}")
            {
                // End of the current section
                if (subSection != null)
                {
                    subSection = null;
                }
                else if (mainSection != null)
                {
                    mainSection = null;
                }
            }
        }
    }

    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetConfigData()
    {
        return configData;
    }
}