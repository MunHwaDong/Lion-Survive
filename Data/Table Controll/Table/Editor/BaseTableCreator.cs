using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class BaseTableCreator : EditorWindow
{
    [MenuItem("Parsing Controll/Rebuild Parsing Table Class")]
    public static void RebuildTableClass()
    {
        
    }
}
public partial class BaseTableCreator : EditorWindow
{
    private const string SCRIPT_TEMPLATE =
        "using System;\n" +
        "using System.Collections;\n" +
        "using System.Collections.Generic;\n" +
        "using UnityEngine;\n" +
        "\n" +
        "public class {TABLE_NAME} : BaseTable\n" +
        "{\n" +
        "   public class {DATA_NAME}\n" +
        "   {\n" +
        "       {DATA_MEMBERS}" +
        "   }\n" +
        "   [Serializable]" +
        "   public class {ROW_NAME}Row\n" +
        "   {\n" +
        "       {DATA_MEMBERS}" +
        "   }\n" +
        "   public override void Parsing(string jsonPath)\n" +
        "   {\n" + 
        "       base.Parsing(jsonPath);\n" +
        "       rows" + "Rows = JsonHelper.FromJson<{ROW_NAME}Row>(json);\n" +
        "       dataDict = ConvertListToDict()\n" +
        "\n" +
        "   private Dictionary<int, {DATA_NAME}> ConvertListToDict()\n" +
        "   {\n" +
        "       Dicrionary<int, {DATA_NAME}> dict = new Dictionary<int, {DATA_NAME}>();\n" +
        "       \n" +
        "       foreach(var row in rows)\n" +
        "       {\n" +
        "            {DATA_NAME} data = new {DATA_NAME}()\n" +
        "            \n" +
        "            int id = row.id;\n" +
        "            \n" +
        "            {DATA_MEMBERS}\n" + 
        
        "            dataDict.TryAdd(id, data);\n" +
        "       }\n" +
        "   }\n" +
        "   public {DATA_NAME} Get{Data_NAME}(int id)\n" +
        "   {\n" + 
        "       if(data.TryGetValue(id, out var tableData))\n" +
        "       {\n" +
        "           return tableData;\n" +
        "       }\n" +
        "       else\n" +
        "           throw new Exception($\"테이블에 없는 ID : {id} 입니다.\")" +
        "   }\n" +
        "   {ROW_NAME}Rows[] rows;\n" +
        "   Dictionary<int, {DATA_NAME}> dataDict = new Dictionary<int, {DATA_NAME}>();\n" +
        "}\n";
}