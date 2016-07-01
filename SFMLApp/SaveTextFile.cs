using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SaveTextFile {

	public List<string> Properties;
	public List<string> Values;

	private int indexread = -1;

	public SaveTextFile() {
		Values = new List<string>();
		Properties = new List<string>();
	}

	public void Save(string file) {
		StreamWriter writer = new StreamWriter(@"" + file);
		for (int i = 0; i < Values.Count; ++i) {
			writer.WriteLine(Properties[i] + "=" + Values[i]);
		}
		writer.Close();
	}

	public void Load(string file) {
		Properties.Clear();
		Values.Clear();
		StreamReader reader = new StreamReader(@"" + file);
		string line = "";
		indexread = -1;
		while ((line = reader.ReadLine()) != null) {
			string[] parts = line.Split('=');
			Properties.Add(parts[0]);
			Values.Add(parts[1]);
		}
		reader.Close();
	}

	private void IndexProperty(string property) {
		indexread = -1;
		for (int i = 0; i < Properties.Count; ++i) {
			if (Properties[i] == property) {
				indexread = i;
				return;
			}
		}
	}

	public string GetPropertyString(string property) {
		if (++indexread >= Properties.Count || Properties[indexread] != property) {
			IndexProperty(property);
		}
		return Values[indexread];
	}

	public int GetPropertyInt(string property) { 
		if (++indexread >= Properties.Count || Properties[indexread] != property) {
			IndexProperty(property);
		}
		return Convert.ToInt32(Values[indexread]);
	}

	public float GetPropertyFloat(string property) {
		if (++indexread >= Properties.Count || Properties[indexread] != property) {
			IndexProperty(property);
		}
		return Convert.ToSingle(Values[indexread]);
	}

	public void AddElement(string property, string value) {
		Properties.Add(property);
		Values.Add(value);
	}

	public void AddElement(string property, int value) {
		Properties.Add(property);
		Values.Add(value.ToString());
	}

	public void AddElement(string property, float value) {
		Properties.Add(property);
		Values.Add(value.ToString());
	}
}
