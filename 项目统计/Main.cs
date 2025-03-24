using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace ProCalc;

public partial class Main : Node2D
{
	static public string rootPath = "";
	static Label labelContent;
	static System.Collections.Generic.List<FileInfo> files = new System.Collections.Generic.List<FileInfo>();
	static GridContainer gridContainer;
	public Timer timer;
	public LineEdit lineEdit;
	public string proPath;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Main.labelContent = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainerSum/BoxContainerSum/LabelContent");
		Main.gridContainer = GetNode<GridContainer>("MarginContainer/VBoxContainer/MarginContainerFileInfo/ScrollContainer/GridContainer");
		this.timer = GetNode<Timer>("Timer");
		this.lineEdit = GetNode<LineEdit>("MarginContainer/VBoxContainer/MarginContainerSearch/BoxContainerInput/LineEdit");
		this.getRootPath();
		this.getFilesByRootPath(Main.rootPath);
		Main.calculateMusicPro();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void getRootPath()
	{
		proPath = Directory.GetCurrentDirectory();
		string[] currentPath = proPath.Split("\\");
		Main.rootPath = currentPath[0] + "\\";
	}

	public void getFilesByRootPath(string path)
	{
		if (!proPath.Contains(path))
		{
			return;
		}
		DirectoryInfo directory = new DirectoryInfo(path);
		DirectoryInfo[] directoryInfos = directory.GetDirectories();
		FileInfo[] fileInfos = directory.GetFiles();
		// Debug.WriteLine(path, directory.GetDirectories());
		foreach (var dir in directoryInfos)
		{
			getFilesByRootPath(dir.FullName);
		}
		foreach (var file in fileInfos)
		{
			var extension = Path.GetExtension(file.FullName);
			if (extension != ".mscz")
			{
				continue;
			}
			Main.files.Add(file);
		}
	}

	static public void calculateMusicPro()
	{
		// 统计文件数量
		var musicCount = 0;
		foreach (var file in Main.files)
		{

			var marginContainerName = new MarginContainer();
			var marginContainerSize = new MarginContainer();
			var labeFileName = new Label();
			var labelFileSize = new Label();
			labeFileName.Text = file.Name;
			labelFileSize.Text = (file.Length / 1024.0).ToString("0.00");
			marginContainerName.AddChild(labeFileName);
			marginContainerSize.AddChild(labelFileSize);
			Main.gridContainer.AddChild(marginContainerName);
			Main.gridContainer.AddChild(marginContainerSize);
			musicCount++;
		}
		labelContent.Text = musicCount.ToString();
	}

	public void _on_line_edit_text_changed(string text)
	{
		// 重置计时器
		this.timer.Start(2.0);
	}

	public void _on_timer_timeout()
	{
		foreach (var child in Main.gridContainer.GetChildren())
		{
			child.QueueFree();
		}
		foreach (var file in Main.files)
		{
			var extension = Path.GetExtension(file.FullName);
			if (extension != ".mscz" || !file.Name.Contains(this.lineEdit.Text))
			{
				continue;
			}
			var marginContainerName = new MarginContainer();
			var marginContainerSize = new MarginContainer();
			var labeFileName = new Label();
			var labelFileSize = new Label();
			labeFileName.Text = file.Name;
			labelFileSize.Text = (file.Length / 1024.0).ToString("0.00");
			marginContainerName.AddChild(labeFileName);
			marginContainerSize.AddChild(labelFileSize);
			Main.gridContainer.AddChild(marginContainerName);
			Main.gridContainer.AddChild(marginContainerSize);
		}
		this.timer.Stop();
	}
}
