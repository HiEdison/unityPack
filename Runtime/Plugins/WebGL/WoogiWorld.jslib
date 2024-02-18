mergeInto(LibraryManager.library, {  
    UnityInitCompleted:function(msg)
    {
       window.UnityInitCompleted();
    }, 
    
    GetSessionStorageData:function(msg)
    {
        var returnStr = window.top.getSessionStorageData(UTF8ToString(msg),1);
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    
    GotoLoginPage:function(msg)
    {
        window.gotoLoginPage();         
    },
    Udp_transport:function(msg)
    {
        window.net_send_msg(UTF8ToString(msg));
    },
});