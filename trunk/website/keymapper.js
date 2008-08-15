function ShowHidetoc(hide) {

    var toc = document.getElementById("toc_container");
    if (toc) {
        if (hide)
            toc.style.display = "none";
        else
            toc.style.display = "";
    }
    var tocanchor = document.getElementById("showhidetocanchor");
    if (tocanchor) {
        if (hide) {
            tocanchor.onclick = function() { return (ShowHidetoc(false)); };
            tocanchor.firstChild.data = "Show";
        }
        else {
            tocanchor.onclick = function() { return (ShowHidetoc(true)); };
            tocanchor.firstChild.data = "Hide";
        }
    }

    //  SetHideShowCookie(hide) ;
    return false;

}

function SetHideShowCookie(hide) {
    setCookie("showfaqtoc", escape(!hide), 30);
}

function setCookie(cookiename, value, expiredays) {
    var expirydate = new Date();
    expirydate.setDate(expirydate.getDate() + expiredays);
    document.cookie = cookiename + "=" + escape(value) +
        ((expiredays == null) ? "" : ";expires=" + expirydate.toGMTString());
}


function getCookie(cookiename) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(cookiename + "=");
        if (c_start != -1) {
            c_start = c_start + cookiename.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}

function load() {

    // ShowHidetoc(getCookie("showfaqtoc")) ;


    // Need to account for all these extra cases:
    // Css is disabled, scripting is disabled
    // Css is enabled, scripting is disabled
    // Css is disabled, scripting is enabled

    // Add in - cookies disabled..

    // By default - ie without scripting - all elements are shown and the text to show them is itself hidden.

    // In Asp.net, can't test for cookies or scripting without redirection. Ugh..


}






