using System;

namespace Xiugou.Entities.Utils
{
    public class EnumUtils
    {
        /// <summary>
        /// Like Enum.TryParse, but won't accept values that aren't in the enum definition.
        /// </summary>
        /// <example>
        /// public enum TheEnum : byte
        /// {
        ///  A = 1,
        ///  B = 3,
        /// }
        /// ...
        /// 
        /// TheEnum? result = StrictTryParseEnum&lt;TheEnum&gt;("11");
        /// 
        /// ==> result==null
        /// Normal Enum.TryParse would just map whatever you sent onto a byte without complaint.  This one returns null in that case.
        /// </example>
        public static TEnum? StrictTryParseEnum<TEnum>(string value) where TEnum : struct
        {
            TEnum result;
            var res = Enum.TryParse(value, out result);
            if (res && Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }
            return null;
        }
    }
}
