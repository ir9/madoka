using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	public class AppConfigTag
	{
		public string TagName { get; set; }
		public int TagID { get; set; }
		public string[] FontPathList { get; set; }
	};

	public class AppConfig
	{
		public string[] RootFontDirList { get; set; }
		public AppConfigTag[] TagList { get; set; } = new AppConfigTag[] { };
	};

	class AppConfigSaverCtrl
	{
		private readonly DataSet1 _dataSet;
		private readonly ModelMain _model;
		private Task _prevTask = Task.FromResult(0);

		public AppConfigSaverCtrl(DataSet1 dataSet, ModelMain model)
		{
			_dataSet = dataSet;
			_model = model;
		}

		public Task WriteAsync(bool force)
		{
			if (!_model.appState.HasFlag(AppState.CONFIG_LOADED))
				return null;

			if (!_prevTask.IsCompleted)
			{
				if (!force)
					return null;
				_prevTask.Wait(2000);
			}

			Task task = new Task(WriteInternal);
			_prevTask = task;
			TaskCtrl ctrl = new TaskCtrl(_model);
			ctrl.StartTask(task);

			return task;
		}

		private void WriteInternal()
		{
			AppConfig config;

			using (_dataSet.GetReadLocker())
			{
				config = new AppConfig()
				{
					RootFontDirList = (from r in _dataSet.RootFontDirTable select r.path).ToArray(),
					TagList = BuildTagList(),
				};
			}

			string tmpPath = Path.Combine(K.AppConfigPath, K.ConfigTmpFileName);
			using (StreamWriter w = new StreamWriter(tmpPath))
			{
				XmlSerializer xs = new XmlSerializer(typeof(AppConfig));
				xs.Serialize(w, config);
			}

			string backPath = Path.Combine(K.AppConfigPath, K.ConfigBackFileName);
			string mainPath = Path.Combine(K.AppConfigPath, K.ConfigFileName);

			File.Delete(backPath); // remove the back file if exists
			try
			{
				File.Move(mainPath, backPath);
			}
			catch (IOException)
			{
				// pass // mainPath が無くてもまぁ…
			}
			File.Move(tmpPath, mainPath);
			File.Delete(backPath);
		}

		private AppConfigTag[] BuildTagList()
		{
			AppConfigTag BuildFromTag(DataSet1.TagTableRow row)
			{
				var it = from   fontId in row.tagObj.FontIdList.AsParallel()
						 let    fontRow = _dataSet.FontFileTable.FindByidSafe<DataSet1.FontFileTableRow>(fontId)
						 where  fontRow != null
						 select fontRow.filepath;
				string[] fontPathList = it.ToArray();

				return new AppConfigTag()
				{
					TagName = row.name,
					TagID = row.id,
					FontPathList = fontPathList,
				};
			}

			return _dataSet.TagTable.Select(BuildFromTag).ToArray();
		}
	}

	class AppConfigLoaderCtrl
	{
		private readonly ModelMain _model;

		public AppConfigLoaderCtrl(ModelMain model)
		{
			_model = model;
		}

		public Task<AppConfig> Load()
		{
			Task<AppConfig> task = new Task<AppConfig>(LoadInternal);
			TaskCtrl ctrl = new TaskCtrl(_model);
			ctrl.StartTask(task);

			return task;
		}

		private AppConfig LoadInternal()
		{
			string tmpPath  = Path.Combine(K.AppConfigPath, K.ConfigTmpFileName);
			string backPath = Path.Combine(K.AppConfigPath, K.ConfigBackFileName);
			string mainPath = Path.Combine(K.AppConfigPath, K.ConfigFileName);

			string[] order = {
				mainPath, backPath, tmpPath
			};

			foreach(string path in order)
			{
				AppConfig ret = LoadFromXMLFile(mainPath);
				if (ret != null)
					return ret;
			}
			return null;
		}

		private AppConfig LoadFromXMLFile(string path)
		{
			try
			{
				using (StreamReader reader = new StreamReader(path))
				{
					XmlSerializer xs = new XmlSerializer(typeof(AppConfig));
					return (AppConfig)xs.Deserialize(reader);
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	};
}
