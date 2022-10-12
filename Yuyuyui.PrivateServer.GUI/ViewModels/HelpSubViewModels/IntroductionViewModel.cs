using System;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class IntroductionViewModel : HelpSubViewModelBase
{
    public Uri MdP1 => MarkdownDocuments("introduction", 1);
    public Uri MdP2 => MarkdownDocuments("introduction", 2);
    public Uri MdP3 => MarkdownDocuments("introduction", 3);
}