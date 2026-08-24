using System.Data;
using System.IO;
using System.Text;
using Data;
using Excel;
using UnityEngine;

public class ExcelTool
{
	private static ExcelTool instance;

	public static ExcelTool GetInstance()
	{
		if (instance == null)
		{
			instance = new ExcelTool();
		}
		return instance;
	}

	public static Item[] CreateItemArrayWithTxt(string kindsname)
	{
		FileInfo[] files = new DirectoryInfo(Application.persistentDataPath + "/" + kindsname).GetFiles();
		int num = files.Length;
		Item[] array = new Item[num];
		for (int i = 0; i < num; i++)
		{
			if (files[i].Name.EndsWith(".txt"))
			{
				string[] array2 = File.ReadAllLines(files[i].FullName, Encoding.UTF8);
				Item item = new Item();
				item.relayname = array2[0].Split(':')[1];
				item.ChannelNum = uint.Parse(array2[1].Split(':')[1]);
				item.ip = array2[2].Split(':')[1];
				item.port = uint.Parse(array2[3].Split(':')[1]);
				item.allopen = array2[4].Split(':')[1];
				item.allclose = array2[5].Split(':')[1];
				item.firstOpen = array2[6].Split(':')[1];
				item.firstClose = array2[7].Split(':')[1];
				item.secondopen = array2[8].Split(':')[1];
				item.secondclose = array2[9].Split(':')[1];
				item.thirdopen = array2[10].Split(':')[1];
				item.thirdclose = array2[11].Split(':')[1];
				item.forthopen = array2[12].Split(':')[1];
				item.forthclose = array2[13].Split(':')[1];
				item.fifthopen = array2[14].Split(':')[1];
				item.fifthclose = array2[15].Split(':')[1];
				item.sixopen = array2[16].Split(':')[1];
				item.sixclose = array2[17].Split(':')[1];
				item.seventhopen = array2[18].Split(':')[1];
				item.seventhclose = array2[19].Split(':')[1];
				item.eightopen = array2[20].Split(':')[1];
				item.eightclose = array2[21].Split(':')[1];
				array[i] = item;
			}
		}
		return array;
	}

	public static Item[] CreateItemArrayWithExcel(string filePath)
	{
		int columnNum = 0;
		int rowNum = 0;
		DataRowCollection dataRowCollection = ReadExcel(filePath, ref columnNum, ref rowNum, 0);
		Item[] array = new Item[rowNum - 1];
		for (int i = 1; i < rowNum; i++)
		{
			Item item = new Item();
			item.relayname = dataRowCollection[i][0].ToString();
			item.ChannelNum = uint.Parse(dataRowCollection[i][1].ToString());
			item.ip = dataRowCollection[i][2].ToString();
			item.port = uint.Parse(dataRowCollection[i][3].ToString());
			item.allopen = dataRowCollection[i][4].ToString();
			item.allclose = dataRowCollection[i][5].ToString();
			item.firstOpen = dataRowCollection[i][6].ToString();
			item.firstClose = dataRowCollection[i][7].ToString();
			item.secondopen = dataRowCollection[i][8].ToString();
			item.secondclose = dataRowCollection[i][9].ToString();
			item.thirdopen = dataRowCollection[i][10].ToString();
			item.thirdclose = dataRowCollection[i][11].ToString();
			item.forthopen = dataRowCollection[i][12].ToString();
			item.forthclose = dataRowCollection[i][13].ToString();
			item.fifthopen = dataRowCollection[i][14].ToString();
			item.fifthclose = dataRowCollection[i][15].ToString();
			item.sixopen = dataRowCollection[i][16].ToString();
			item.sixclose = dataRowCollection[i][17].ToString();
			item.seventhopen = dataRowCollection[i][18].ToString();
			item.seventhclose = dataRowCollection[i][19].ToString();
			item.eightopen = dataRowCollection[i][20].ToString();
			item.eightclose = dataRowCollection[i][21].ToString();
			array[i - 1] = item;
		}
		return array;
	}

	public static PhotoItem[] CreatePhotoItemWithExcel(string filePath)
	{
		int columnNum = 0;
		int rowNum = 0;
		DataRowCollection dataRowCollection = ReadExcel(filePath, ref columnNum, ref rowNum, 1);
		PhotoItem[] array = new PhotoItem[rowNum - 1];
		for (int i = 1; i < rowNum; i++)
		{
			PhotoItem photoItem = new PhotoItem();
			photoItem.IP = dataRowCollection[i][0].ToString();
			array[i - 1] = photoItem;
		}
		return array;
	}

	private static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum, int num)
	{
		using FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
		DataSet dataSet = ExcelReaderFactory.CreateBinaryReader(fileStream).AsDataSet();
		columnNum = dataSet.Tables[num].Columns.Count;
		rowNum = dataSet.Tables[num].Rows.Count;
		return dataSet.Tables[num].Rows;
	}
}
