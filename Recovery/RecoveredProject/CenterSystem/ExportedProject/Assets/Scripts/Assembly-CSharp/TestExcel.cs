using System.Data;
using System.IO;
using Excel;
using UnityEngine;

public class TestExcel : MonoBehaviour
{
	private void Start()
	{
		DataRowCollection dataRowCollection = ReadExcel(Application.dataPath + "/Resources/DataAssets/Excels/item.xls");
		for (int i = 1; i < dataRowCollection.Count; i++)
		{
		}
	}

	private static DataRowCollection ReadExcel(string _path, int _sheetIndex = 0)
	{
		return ExcelReaderFactory.CreateOpenXmlReader(File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read)).AsDataSet().Tables[_sheetIndex].Rows;
	}

	private static DataRowCollection ReadExcel(string _path, string _sheetName)
	{
		return ExcelReaderFactory.CreateOpenXmlReader(File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read)).AsDataSet().Tables[_sheetName].Rows;
	}
}
