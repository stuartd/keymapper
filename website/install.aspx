<%@ Page Language="C#" MasterPageFile="keymapper.master" AutoEventWireup="true" CodeFile="install.aspx.cs"
    Inherits="KeyMapperInstall" Title="Install Key Mapper" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script language="JavaScript" type="text/javascript">
<!--
runtimeVersion = "2.0.0";

function Initialize()
{
  if (HasRuntimeVersion(runtimeVersion))
  {
  prerequisites.style.display="none" ;
   not_installed_or_not_IE.style.display="none";
   framework_installed.style.display="block" ;
   
  }
}
function HasRuntimeVersion(v)
{
  var va = GetVersion(v);
  var i;
  var a = navigator.userAgent.match(/\.NET CLR [0-9.]+/g);
  if (a != null)
    for (i = 0; i < a.length; ++i)
      if (CompareVersions(va, GetVersion(a[i])) <= 0)
		return true;
  return false;
}
function GetVersion(v)
{
  var a = v.match(/([0-9]+)\.([0-9]+)\.([0-9]+)/i);
    return a.slice(1);
}
function CompareVersions(v1, v2)
{
  for (i = 0; i < v1.length; ++i)
  {
    var n1 = new Number(v1[i]);
    var n2 = new Number(v2[i]);
    if (n1 < n2)
      return -1;
    if (n1 > n2)
      return 1;
  }
  return 0;
}

-->
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <div id="prerequisites">
        The .NET Framework 2.0 is required to run this program.</div>
    <br />
    <div id="not_installed_or_not_IE">
        If the framework is already installed, you can <a href="download.ashx?file=KeyMapper.application">launch</a>
        the application now. Otherwise, <a id="A1" href="download.ashx?file=setup.exe">click here</a> to run
        setup which will install the framework and run the application.
    </div>
    <div id="framework_installed" style="display: none;">
        <a href="KeyMapper.application">Click here to install and run the program</a>
    </div>
    <br />
    If you prefer or if you run a 64 bit version of Windows, you can download one of the pre-built versions of Key Mapper by following the Downloads link above. These won't update automatically, but will run slightly faster.
</asp:Content>
