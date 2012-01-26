using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace NContrib {

    public static class MimeHelper {

        public static int MimeSampleSize = 256;

        public static string DefaultMimeType = "application/octet-stream";

        /// <summary>
        /// Custom file extension and MIME types to add or override to the built-in types
        /// </summary>
        public static Dictionary<string, string> UserTypes { get; set; }

        /// <summary>
        /// Comprehensive list of known MIME types
        /// </summary>
        private static readonly Dictionary<string, string> BuiltInTypes;

        public static readonly Dictionary<string, string> UrlmonOverrides;

        static MimeHelper() {
            UserTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            UrlmonOverrides = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"image/pjpeg", "image/jpeg"},
                {"image/x-png", "image/png"},
            };
            
            BuiltInTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {".3dm", "x-world/x-3dmf"},
                {".3dmf", "x-world/x-3dmf"},
                {".a", DefaultMimeType},
                {".aab", "application/x-authorware-bin"},
                {".aam", "application/x-authorware-map"},
                {".aas", "application/x-authorware-seg"},
                {".abc", "text/vnd.abc"},
                {".acgi", "text/html"},
                {".afl", "video/animaflex"},
                {".ai", "application/postscript"},
                {".aif", "audio/aiff"},
                {".aifc", "audio/aiff"},
                {".aiff", "audio/aiff"},
                {".aim", "application/x-aim"},
                {".aip", "text/x-audiosoft-intra"},
                {".ani", "application/x-navi-animation"},
                {".aos", "application/x-nokia-9000-communicator-add-on-software"},
                {".aps", "application/mime"},
                {".arc", DefaultMimeType},
                {".arj", "application/arj"},
                {".art", "image/x-jg"},
                {".asf", "video/x-ms-asf"},
                {".asm", "text/x-asm"},
                {".asp", "text/asp"},
                {".asx", "video/x-ms-asf"},
                {".au", "audio/basic"},
                {".avi", "video/avi"},
                {".avs", "video/avs-video"},
                {".bcpio", "application/x-bcpio"},
                {".bin", DefaultMimeType},
                {".bm", "image/bmp"},
                {".bmp", "image/bmp"},
                {".boo", "application/book"},
                {".book", "application/book"},
                {".boz", "application/x-bzip2"},
                {".bsh", "application/x-bsh"},
                {".bz", "application/x-bzip"},
                {".bz2", "application/x-bzip2"},
                {".c", "text/plain"},
                {".c++", "text/plain"},
                {".cat", "application/vnd.ms-pki.seccat"},
                {".cc", "text/plain"},
                {".ccad", "application/clariscad"},
                {".cco", "application/x-cocoa"},
                {".cdf", "application/cdf"},
                {".cer", "application/pkix-cert"},
                {".cha", "application/x-chat"},
                {".chat", "application/x-chat"},
                {".class", "application/java"},
                {".com", DefaultMimeType},
                {".conf", "text/plain"},
                {".cpio", "application/x-cpio"},
                {".cpp", "text/x-c"},
                {".cpt", "application/x-cpt"},
                {".crl", "application/pkcs-crl"},
                {".crt", "application/pkix-cert"},
                {".csh", "application/x-csh"},
                {".css", "text/css"},
                {".cxx", "text/plain"},
                {".dcr", "application/x-director"},
                {".deepv", "application/x-deepv"},
                {".def", "text/plain"},
                {".der", "application/x-x509-ca-cert"},
                {".dif", "video/x-dv"},
                {".dir", "application/x-director"},
                {".dl", "video/dl"},
                {".doc", "application/msword"},
                {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".dot", "application/msword"},
                {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
                {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
                {".dp", "application/commonground"},
                {".drw", "application/drafting"},
                {".dump", DefaultMimeType},
                {".dv", "video/x-dv"},
                {".dvi", "application/x-dvi"},
                {".dwf", "model/vnd.dwf"},
                {".dwg", "image/vnd.dwg"},
                {".dxf", "image/vnd.dwg"},
                {".dxr", "application/x-director"},
                {".el", "text/x-script.elisp"},
                {".elc", "application/x-elc"},
                {".env", "application/x-envoy"},
                {".eps", "application/postscript"},
                {".es", "application/x-esrehber"},
                {".etx", "text/x-setext"},
                {".evy", "application/envoy"},
                {".exe", DefaultMimeType},
                {".f", "text/plain"},
                {".f77", "text/x-fortran"},
                {".f90", "text/plain"},
                {".fdf", "application/vnd.fdf"},
                {".fif", "image/fif"},
                {".fli", "video/fli"},
                {".flo", "image/florian"},
                {".flx", "text/vnd.fmi.flexstor"},
                {".fmf", "video/x-atomic3d-feature"},
                {".for", "text/x-fortran"},
                {".fpx", "image/vnd.fpx"},
                {".frl", "application/freeloader"},
                {".funk", "audio/make"},
                {".g", "text/plain"},
                {".g3", "image/g3fax"},
                {".gif", "image/gif"},
                {".gl", "video/gl"},
                {".gsd", "audio/x-gsm"},
                {".gsm", "audio/x-gsm"},
                {".gsp", "application/x-gsp"},
                {".gss", "application/x-gss"},
                {".gtar", "application/x-gtar"},
                {".gz", "application/x-gzip"},
                {".gzip", "application/x-gzip"},
                {".h", "text/plain"},
                {".hdf", "application/x-hdf"},
                {".help", "application/x-helpfile"},
                {".hgl", "application/vnd.hp-hpgl"},
                {".hh", "text/plain"},
                {".hlb", "text/x-script"},
                {".hlp", "application/hlp"},
                {".hpg", "application/vnd.hp-hpgl"},
                {".hpgl", "application/vnd.hp-hpgl"},
                {".hqx", "application/binhex"},
                {".hta", "application/hta"},
                {".htc", "text/x-component"},
                {".htm", "text/html"},
                {".html", "text/html"},
                {".htmls", "text/html"},
                {".htt", "text/webviewhtml"},
                {".htx", "text/html"},
                {".ice", "x-conference/x-cooltalk"},
                {".ico", "image/x-icon"},
                {".idc", "text/plain"},
                {".ief", "image/ief"},
                {".iefs", "image/ief"},
                {".iges", "application/iges"},
                {".igs", "application/iges"},
                {".ima", "application/x-ima"},
                {".imap", "application/x-httpd-imap"},
                {".inf", "application/inf"},
                {".ins", "application/x-internett-signup"},
                {".ip", "application/x-ip2"},
                {".isu", "video/x-isvideo"},
                {".it", "audio/it"},
                {".iv", "application/x-inventor"},
                {".ivr", "i-world/i-vrml"},
                {".ivy", "application/x-livescreen"},
                {".jam", "audio/x-jam"},
                {".jav", "text/plain"},
                {".java", "text/plain"},
                {".jcm", "application/x-java-commerce"},
                {".jfif", "image/jpeg"},
                {".jfif-tbnl", "image/jpeg"},
                {".jpe", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".jpg", "image/jpeg"},
                {".jps", "image/x-jps"},
                {".js", "application/x-javascript"},
                {".jut", "image/jutvision"},
                {".kar", "audio/midi"},
                {".ksh", "application/x-ksh"},
                {".la", "audio/nspaudio"},
                {".lam", "audio/x-liveaudio"},
                {".latex", "application/x-latex"},
                {".lha", DefaultMimeType},
                {".lhx", DefaultMimeType},
                {".list", "text/plain"},
                {".lma", "audio/nspaudio"},
                {".log", "text/plain"},
                {".lsp", "application/x-lisp"},
                {".lst", "text/plain"},
                {".lsx", "text/x-la-asf"},
                {".ltx", "application/x-latex"},
                {".lzh", DefaultMimeType},
                {".lzx", DefaultMimeType},
                {".m", "text/plain"},
                {".m1v", "video/mpeg"},
                {".m2a", "audio/mpeg"},
                {".m2v", "video/mpeg"},
                {".m3u", "audio/x-mpequrl"},
                {".m4a", "audio/mp4"},
                {".m4v", "video/mp4"},
                {".man", "application/x-troff-man"},
                {".map", "application/x-navimap"},
                {".mar", "text/plain"},
                {".mbd", "application/mbedlet"},
                {".mc$", "application/x-magic-cap-package-1.0"},
                {".mcd", "application/mcad"},
                {".mcf", "text/mcf"},
                {".mcp", "application/netmc"},
                {".me", "application/x-troff-me"},
                {".mht", "message/rfc822"},
                {".mhtml", "message/rfc822"},
                {".mid", "audio/midi"},
                {".midi", "audio/midi"},
                {".mif", "application/x-mif"},
                {".mime", "message/rfc822"},
                {".mjf", "audio/x-vnd.audioexplosion.mjuicemediafile"},
                {".mjpg", "video/x-motion-jpeg"},
                {".mm", "application/base64"},
                {".mme", "application/base64"},
                {".mod", "audio/mod"},
                {".moov", "video/quicktime"},
                {".mov", "video/quicktime"},
                {".movie", "video/x-sgi-movie"},
                {".mp2", "audio/mpeg"},
                {".mp3", "audio/mpeg"},
                {".mp4", "application/mp4"},
                {".mpa", "audio/mpeg"},
                {".mpc", "application/x-project"},
                {".mpe", "video/mpeg"},
                {".mpeg", "video/mpeg"},
                {".mpg", "video/mpeg"},
                {".mpga", "audio/mpeg"},
                {".mpp", "application/vnd.ms-project"},
                {".mpt", "application/vnd.ms-project"},
                {".mpv", "application/vnd.ms-project"},
                {".mpx", "application/vnd.ms-project"},
                {".mrc", "application/marc"},
                {".ms", "application/x-troff-ms"},
                {".mv", "video/x-sgi-movie"},
                {".my", "audio/make"},
                {".mzz", "application/x-vnd.audioexplosion.mzz"},
                {".nap", "image/naplps"},
                {".naplps", "image/naplps"},
                {".nc", "application/x-netcdf"},
                {".ncm", "application/vnd.nokia.configuration-message"},
                {".nif", "image/x-niff"},
                {".niff", "image/x-niff"},
                {".nix", "application/x-mix-transfer"},
                {".nsc", "application/x-conference"},
                {".nvd", "application/x-navidoc"},
                {".o", DefaultMimeType},
                {".oda", "application/oda"},
                {".omc", "application/x-omc"},
                {".omcd", "application/x-omcdatamaker"},
                {".omcr", "application/x-omcregerator"},
                {".p", "text/x-pascal"},
                {".p10", "application/pkcs10"},
                {".p12", "application/pkcs-12"},
                {".p7a", "application/x-pkcs7-signature"},
                {".p7c", "application/pkcs7-mime"},
                {".p7m", "application/pkcs7-mime"},
                {".p7r", "application/x-pkcs7-certreqresp"},
                {".p7s", "application/pkcs7-signature"},
                {".part", "application/pro_eng"},
                {".pas", "text/pascal"},
                {".pbm", "image/x-portable-bitmap"},
                {".pcl", "application/vnd.hp-pcl"},
                {".pct", "image/x-pict"},
                {".pcx", "image/x-pcx"},
                {".pdb", "chemical/x-pdb"},
                {".pdf", "application/pdf"},
                {".pfunk", "audio/make"},
                {".pgm", "image/x-portable-greymap"},
                {".pic", "image/pict"},
                {".pict", "image/pict"},
                {".pkg", "application/x-newton-compatible-pkg"},
                {".pko", "application/vnd.ms-pki.pko"},
                {".pl", "text/plain"},
                {".plx", "application/x-pixclscript"},
                {".pm", "image/x-xpixmap"},
                {".pm4", "application/x-pagemaker"},
                {".pm5", "application/x-pagemaker"},
                {".png", "image/png"},
                {".pnm", "application/x-portable-anymap"},
                {".pot", "application/vnd.ms-powerpoint"},
                {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
                {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
                {".pov", "model/x-pov"},
                {".ppa", "application/vnd.ms-powerpoint"},
                {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
                {".ppm", "image/x-portable-pixmap"},
                {".pps", "application/vnd.ms-powerpoint"},
                {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
                {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
                {".ppt", "application/vnd.ms-powerpoint"},
                {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
                {".ppz", "application/vnd.ms-powerpoint"},
                {".pre", "application/x-freelance"},
                {".prt", "application/pro_eng"},
                {".ps", "application/postscript"},
                {".psd", DefaultMimeType},
                {".pvu", "paleovu/x-pv"},
                {".pwz", "application/vnd.ms-powerpoint"},
                {".py", "text/x-script.phyton"},
                {".pyc", "applicaiton/x-bytecode.python"},
                {".qcp", "audio/vnd.qcelp"},
                {".qd3", "x-world/x-3dmf"},
                {".qd3d", "x-world/x-3dmf"},
                {".qif", "image/x-quicktime"},
                {".qt", "video/quicktime"},
                {".qtc", "video/x-qtc"},
                {".qti", "image/x-quicktime"},
                {".qtif", "image/x-quicktime"},
                {".ra", "audio/x-pn-realaudio"},
                {".ram", "audio/x-pn-realaudio"},
                {".ras", "application/x-cmu-raster"},
                {".rast", "image/cmu-raster"},
                {".rexx", "text/x-script.rexx"},
                {".rf", "image/vnd.rn-realflash"},
                {".rgb", "image/x-rgb"},
                {".rm", "application/vnd.rn-realmedia"},
                {".rmi", "audio/mid"},
                {".rmm", "audio/x-pn-realaudio"},
                {".rmp", "audio/x-pn-realaudio"},
                {".rng", "application/ringing-tones"},
                {".rnx", "application/vnd.rn-realplayer"},
                {".roff", "application/x-troff"},
                {".rp", "image/vnd.rn-realpix"},
                {".rpm", "audio/x-pn-realaudio-plugin"},
                {".rt", "text/richtext"},
                {".rtf", "text/richtext"},
                {".rtx", "text/richtext"},
                {".rv", "video/vnd.rn-realvideo"},
                {".s", "text/x-asm"},
                {".s3m", "audio/s3m"},
                {".saveme", DefaultMimeType},
                {".sbk", "application/x-tbook"},
                {".scm", "application/x-lotusscreencam"},
                {".sdml", "text/plain"},
                {".sdp", "application/sdp"},
                {".sdr", "application/sounder"},
                {".sea", "application/sea"},
                {".set", "application/set"},
                {".sgm", "text/sgml"},
                {".sgml", "text/sgml"},
                {".sh", "application/x-sh"},
                {".shar", "application/x-shar"},
                {".shtml", "text/html"},
                {".sid", "audio/x-psid"},
                {".sit", "application/x-sit"},
                {".skd", "application/x-koan"},
                {".skm", "application/x-koan"},
                {".skp", "application/x-koan"},
                {".skt", "application/x-koan"},
                {".sl", "application/x-seelogo"},
                {".smi", "application/smil"},
                {".smil", "application/smil"},
                {".snd", "audio/basic"},
                {".sol", "application/solids"},
                {".spc", "text/x-speech"},
                {".spl", "application/futuresplash"},
                {".spr", "application/x-sprite"},
                {".sprite", "application/x-sprite"},
                {".src", "application/x-wais-source"},
                {".ssi", "text/x-server-parsed-html"},
                {".ssm", "application/streamingmedia"},
                {".sst", "application/vnd.ms-pki.certstore"},
                {".step", "application/step"},
                {".stl", "application/sla"},
                {".stp", "application/step"},
                {".sv4cpio", "application/x-sv4cpio"},
                {".sv4crc", "application/x-sv4crc"},
                {".svf", "image/vnd.dwg"},
                {".svr", "application/x-world"},
                {".swf", "application/x-shockwave-flash"},
                {".t", "application/x-troff"},
                {".talk", "text/x-speech"},
                {".tar", "application/x-tar"},
                {".tbk", "application/toolbook"},
                {".tcl", "application/x-tcl"},
                {".tcsh", "text/x-script.tcsh"},
                {".tex", "application/x-tex"},
                {".texi", "application/x-texinfo"},
                {".texinfo", "application/x-texinfo"},
                {".text", "text/plain"},
                {".tgz", "application/x-compressed"},
                {".tif", "image/tiff"},
                {".tiff", "image/tiff"},
                {".tr", "application/x-troff"},
                {".tsi", "audio/tsp-audio"},
                {".tsp", "application/dsptype"},
                {".tsv", "text/tab-separated-values"},
                {".turbot", "image/florian"},
                {".txt", "text/plain"},
                {".uil", "text/x-uil"},
                {".uni", "text/uri-list"},
                {".unis", "text/uri-list"},
                {".unv", "application/i-deas"},
                {".uri", "text/uri-list"},
                {".uris", "text/uri-list"},
                {".ustar", "application/x-ustar"},
                {".uu", DefaultMimeType},
                {".uue", "text/x-uuencode"},
                {".vcd", "application/x-cdlink"},
                {".vcs", "text/x-vcalendar"},
                {".vda", "application/vda"},
                {".vdo", "video/vdo"},
                {".vew", "application/groupwise"},
                {".viv", "video/vivo"},
                {".vivo", "video/vivo"},
                {".vmd", "application/vocaltec-media-desc"},
                {".vmf", "application/vocaltec-media-file"},
                {".voc", "audio/voc"},
                {".vos", "video/vosaic"},
                {".vox", "audio/voxware"},
                {".vqe", "audio/x-twinvq-plugin"},
                {".vqf", "audio/x-twinvq"},
                {".vql", "audio/x-twinvq-plugin"},
                {".vrml", "application/x-vrml"},
                {".vrt", "x-world/x-vrt"},
                {".vsd", "application/x-visio"},
                {".vst", "application/x-visio"},
                {".vsw", "application/x-visio"},
                {".w60", "application/wordperfect6.0"},
                {".w61", "application/wordperfect6.1"},
                {".w6w", "application/msword"},
                {".wav", "audio/wav"},
                {".wb1", "application/x-qpro"},
                {".wbmp", "image/vnd.wap.wbmp"},
                {".web", "application/vnd.xara"},
                {".wiz", "application/msword"},
                {".wk1", "application/x-123"},
                {".wmf", "windows/metafile"},
                {".wml", "text/vnd.wap.wml"},
                {".wmlc", "application/vnd.wap.wmlc"},
                {".wmls", "text/vnd.wap.wmlscript"},
                {".wmlsc", "application/vnd.wap.wmlscriptc"},
                {".word", "application/msword"},
                {".wp", "application/wordperfect"},
                {".wp5", "application/wordperfect"},
                {".wp6", "application/wordperfect"},
                {".wpd", "application/wordperfect"},
                {".wq1", "application/x-lotus"},
                {".wri", "application/mswrite"},
                {".wrl", "application/x-world"},
                {".wrz", "x-world/x-vrml"},
                {".wsc", "text/scriplet"},
                {".wsrc", "application/x-wais-source"},
                {".wtk", "application/x-wintalk"},
                {".xbm", "image/x-xbitmap"},
                {".xdr", "video/x-amt-demorun"},
                {".xgz", "xgl/drawing"},
                {".xif", "image/vnd.xiff"},
                {".xl", "application/excel"},
                {".xla", "application/vnd.ms-excel"},
                {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
                {".xlb", "application/vnd.ms-excel"},
                {".xlc", "application/vnd.ms-excel"},
                {".xld", "application/vnd.ms-excel"},
                {".xlk", "application/vnd.ms-excel"},
                {".xll", "application/vnd.ms-excel"},
                {".xlm", "application/vnd.ms-excel"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
                {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                {".xlt", "application/vnd.ms-excel"},
                {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
                {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
                {".xlv", "application/vnd.ms-excel"},
                {".xlw", "application/vnd.ms-excel"},
                {".xm", "audio/xm"},
                {".xml", "application/xml"},
                {".xmz", "xgl/movie"},
                {".xpix", "application/x-vnd.ls-xpix"},
                {".xpm", "image/xpm"},
                {".x-png", "image/png"},
                {".xsr", "video/x-amt-showrun"},
                {".xwd", "image/x-xwd"},
                {".xyz", "chemical/x-pdb"},
                {".z", "application/x-compressed"},
                {".zip", "application/zip"},
                {".zoo", DefaultMimeType},
                {".zsh", "text/x-script.zsh"}
            };
        }

        public static string GetMimeFromFileName(string filename) {

            if (string.IsNullOrEmpty(filename))
                return DefaultMimeType;

            var extension = Path.GetExtension(filename);

            if (string.IsNullOrEmpty(extension))
                return DefaultMimeType;

            string mimetype;

            if (!UserTypes.TryGetValue(extension, out mimetype))
                BuiltInTypes.TryGetValue(extension, out mimetype);

            return mimetype ?? DefaultMimeType;
        }

        /// <summary>
        /// Uses IE Mime Type Detection to find the MIME type from bytes. Supports 26 MIME types.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms775147(v=vs.85).aspx"/>
        /// <param name="pBC">A pointer to the IBindCtx interface. Can be set to NULL.</param>
        /// <param name="pwzUrl">A pointer to a string value that contains the URL of the data. Can be set to NULL if pBuffer contains the data to be sniffed.</param>
        /// <param name="pBuffer">A pointer to the buffer that contains the data to be sniffed. Can be set to NULL if pwzUrl contains a valid URL. </param>
        /// <param name="cbSize">An unsigned long integer value that contains the size of the buffer. </param>
        /// <param name="pwzMimeProposed">A pointer to a string value that contains the proposed MIME type. This value is authoritative if nothing else can be determined about type. Can be set to NULL.</param>
        /// <param name="dwMimeFlags"></param>
        /// <param name="ppwzMimeOut">The address of a string value that receives the suggested MIME type. </param>
        /// <param name="dwReserverd">Reserved. Must be set to 0.</param>
        /// <returns></returns>
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static uint FindMimeFromData(
            uint pBC,
            [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            uint cbSize,
            [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
            uint dwMimeFlags,
            out uint ppwzMimeOut,
            uint dwReserverd
        );

        /// <summary>
        /// Use MIME magic to find the MIME type of this data block.
        /// This should be the first 256 bytes of a file
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetMimeFromBytes(this byte[] data) {
            try {
                uint mimeType;
                FindMimeFromData(0, null, data, (uint)MimeSampleSize, null, 0, out mimeType, 0);

                var mimePointer = new IntPtr(mimeType);
                var mime = Marshal.PtrToStringUni(mimePointer);
                Marshal.FreeCoTaskMem(mimePointer);

                if (mime == null) return DefaultMimeType;

                UrlmonOverrides.TryGetValue(mime, out mime);

                return mime ?? DefaultMimeType;
            }
            catch {
                return DefaultMimeType;
            }
        }

        /// <summary>
        /// Use MIME magic to find the MIME type of this file
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string GetMimeFromBytes(this FileInfo fileInfo) {
            using (var fs = fileInfo.OpenRead())
                return fs.GetMimeFromBytes();
        }

        public static string GetMimeFromFileName(this FileInfo fileInfo) {
            return GetMimeFromFileName(fileInfo.Name);
        }

        /// <summary>
        /// Use MIME magic to find the mime of the file at this path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMimeFromBytes(string path) {
            using (var reader = File.OpenRead(path))
                return reader.GetMimeFromBytes();
        }

        /// <summary>
        /// Use MIME magic to find the MIME type for this stream.
        /// </summary>
        /// <remarks>
        /// 256 bytes (or the total number of bytes in the stream if 256 are not available)
        /// are read from the current stream location to detect the MIME type.
        /// If the stream support seeking, the stream position will be moved back after
        /// the sample is taken.
        /// </remarks>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMimeFromBytes(this Stream stream) {

            var buffer = new byte[MimeSampleSize];

            var readLength = stream.Length >= MimeSampleSize
                ? MimeSampleSize
                : (int)stream.Length;

            stream.Read(buffer, 0, readLength);

            if (stream.CanSeek)
                stream.Seek(readLength * -1, SeekOrigin.Current);

            return buffer.GetMimeFromBytes();
        }
    }
}
