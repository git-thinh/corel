using System;
using System.Collections.Generic;
using System.Text;

namespace corel
{
    public enum JOB_TYPE
    {
        NONE,
        MESSAGE,
        REQUEST_URL,
        LINK,
        FILE_HTTP_CACHE,
        RPC_SERVER,
        RPC_CLIENT,
        WEB_SOCKET_SERVER,
        WEB_SOCKET_CLIENT,
        FILE_WRITE,
        ENGLISH_GOOGLE_TRANSLATER,
        ENGLISH_MEDIA,// CACHE & PLAY: MP3, MP4, YOUTUBE, ...
        ENGLISH_PHONIC, // SPEECH: VOWLE, WORD, ...
    }
}
