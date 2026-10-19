using System;

public class ProgressBarShowData
{
	public string Content = "";

	public ProgressBarType ProcessBarType;

	public Action OnComplete;

	public ProgressBarShowData()
	{
	}

	public ProgressBarShowData(string content, Action onComplete = null, ProgressBarType processBarType = ProgressBarType.Normal)
	{
		Content = content;
		ProcessBarType = processBarType;
		OnComplete = onComplete;
	}
}
