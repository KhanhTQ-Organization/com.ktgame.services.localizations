using System.Collections.Generic;
using System.IO;
using System.Text;
using com.localizations.tmpro_font_generator.editor;
using I2.Loc;
using UnityEditor;
using UnityEngine;

namespace Shared.Localizations.Editor
{
	public class LocalizationConverter : IGoogleImportedCallback
	{
		public void OnImported(List<string> categories)
		{
			var sbTrans = new StringBuilder();
			sbTrans.AppendLine("//This class is auto-generated do not modify it");
			sbTrans.AppendLine("using I2.Loc;");
			sbTrans.AppendLine();
			sbTrans.AppendLine("namespace Shared.Localizations");
			sbTrans.AppendLine("{");
			sbTrans.AppendLine("	public static class Localization");
			sbTrans.AppendLine("	{");
			BuildScriptWithTerms(sbTrans, categories);
			sbTrans.AppendLine("	}");

			var filePath = GetPathToGeneratedScriptLocalization();
			Debug.Log("Generating: " + filePath);

			var fileText = sbTrans + "}";

			File.WriteAllText(filePath, fileText, Encoding.UTF8);
			AssetDatabase.ImportAsset("Assets/Scripts/Generated/Localization.cs");
			AssetDatabase.Refresh();
		}

		private static string GetPathToGeneratedScriptLocalization()
		{
			var folderPath = Application.dataPath + "/Scripts/Generated";
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return $"{folderPath}/Localization.cs";
		}

		private static void BuildScriptWithTerms(StringBuilder builder, List<string> categories)
		{
			foreach (var category in categories)
			{
				var terms = LocalizationManager.GetTermsList(category);
				var categoryTerms = new List<string>(terms);
				for (var i = 0; i < categoryTerms.Count; i++)
				{
					categoryTerms[i] = ScriptToolAdjustTerm(categoryTerms[i]);
				}

				ScriptToolEnumerateDuplicatedTerms(categoryTerms);

				if (category == LanguageSourceData.EmptyCategory)
				{
					builder.AppendLine("		public static class " + ScriptToolAdjustTerm(LanguageSourceData.EmptyCategory, true));
					builder.AppendLine("		{");
				}
				else
				{
					builder.AppendLine("		public static class " + ScriptToolAdjustTerm(category, true));
					builder.AppendLine("		{");
				}

				BuildScriptCategory(builder, category, categoryTerms, terms);

				builder.AppendLine("		}");
				builder.AppendLine();
			}
		}

		private static void BuildScriptCategory(StringBuilder builder, string category, IReadOnlyList<string> adjustedTerms, IReadOnlyList<string> terms)
		{
			if (category == LanguageSourceData.EmptyCategory)
			{
				for (var i = 0; i < terms.Count; ++i)
				{
					builder.AppendLine("			public static string " + adjustedTerms[i] + " => LocalizationManager.GetTranslation(\""
									   + LanguageSourceData.GetKeyFromFullTerm(terms[i]) + "\");");
				}
			}
			else
			{
				for (var i = 0; i < terms.Count; ++i)
				{
					builder.AppendLine("			public static string " + adjustedTerms[i] + " => LocalizationManager.GetTranslation(\"" + category + "/"
									   + LanguageSourceData.GetKeyFromFullTerm(terms[i])
									   + "\");");
				}
			}
		}

		private static void ScriptToolEnumerateDuplicatedTerms(IList<string> adjustedTerms)
		{
			var lastTerm = "$";
			var counter = 1;
			for (int i = 0, imax = adjustedTerms.Count; i < imax; ++i)
			{
				var currentTerm = adjustedTerms[i];
				if (lastTerm == currentTerm || i < imax - 1 && currentTerm == adjustedTerms[i + 1])
				{
					adjustedTerms[i] = adjustedTerms[i] + "_" + counter;
					counter++;
				}
				else
				{
					counter = 1;
				}

				lastTerm = currentTerm;
			}
		}

		private static string ScriptToolAdjustTerm(string term, bool allowFullLength = false)
		{
			term = I2Utils.GetValidTermName(term);

			// C# IDs can't start with a number
			if (I2Utils.NumberChars.IndexOf(term[0]) >= 0)
			{
				term = "_" + term;
			}

			if (!allowFullLength && term.Length > 50)
			{
				term = term.Substring(0, 50);
			}

			// Remove invalid characters
			var chars = term.ToCharArray();
			for (int i = 0, imax = chars.Length; i < imax; ++i)
			{
				if (!IsValidCharacter(chars[i]))
				{
					chars[i] = '_';
				}
			}

			term = new string(chars);
			if (IsCSharpKeyword(term))
			{
				return string.Concat('@', term);
			}

			return term;

			bool IsValidCharacter(char c)
			{
				if (I2Utils.ValidChars.IndexOf(c) >= 0)
				{
					return true;
				}

				return c >= '\u4e00' && c <= '\u9fff'; // Chinese/Japanese characters
			}
		}

		private static bool IsCSharpKeyword(string variableName)
		{
			return variableName == "abstract" || variableName == "as" || variableName == "base" || variableName == "bool" ||
				   variableName == "break" || variableName == "byte" || variableName == "" || variableName == "case" ||
				   variableName == "catch" || variableName == "char" || variableName == "checked" || variableName == "class" ||
				   variableName == "const" || variableName == "continue" || variableName == "decimal" || variableName == "default" ||
				   variableName == "delegate" || variableName == "do" || variableName == "double" || variableName == "else" ||
				   variableName == "enum" || variableName == "event" || variableName == "explicit" || variableName == "extern" ||
				   variableName == "false" || variableName == "finally" || variableName == "fixed" || variableName == "float" ||
				   variableName == "for" || variableName == "foreach" || variableName == "goto" || variableName == "if" ||
				   variableName == "implicit" || variableName == "in" || variableName == "int" || variableName == "interface" ||
				   variableName == "internal" || variableName == "is" || variableName == "lock" || variableName == "long" ||
				   variableName == "namespace" || variableName == "new" || variableName == "null" || variableName == "object" ||
				   variableName == "operator" || variableName == "out" || variableName == "override" || variableName == "params" ||
				   variableName == "private" || variableName == "protected" || variableName == "public" || variableName == "readonly" ||
				   variableName == "ref" || variableName == "return" || variableName == "sbyte" || variableName == "sealed" ||
				   variableName == "short" || variableName == "sizeof" || variableName == "stackalloc" || variableName == "static" ||
				   variableName == "string" || variableName == "struct" || variableName == "switch" || variableName == "this" ||
				   variableName == "throw" || variableName == "true" || variableName == "try" || variableName == "typeof" ||
				   variableName == "uint" || variableName == "ulong" || variableName == "unchecked" || variableName == "unsafe" ||
				   variableName == "short" || variableName == "using" || variableName == "virtual" || variableName == "void" ||
				   variableName == "volatile" || variableName == "while";
		}
	}
}
