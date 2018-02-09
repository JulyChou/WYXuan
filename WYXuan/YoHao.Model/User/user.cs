using System;
using System.Linq;
using System.Text;

namespace YoHao.Model.User
{
    ///<summary>
    ///
    ///</summary>
    public partial class User
    {
        public User()
        {

        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public long Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Name { get; set; }
        /// <summary>
        /// Desc:PassWord
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string PassWord { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Mobile { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Email { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Info { get; set; }

        /// <summary>
        /// Desc:删除标识
        /// Default:b'0'
        /// Nullable:True
        /// </summary>           
        public bool? DelFlag { get; set; }

        /// <summary>
        /// Desc:头像
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Avatar { get; set; }

        /// <summary>
        /// Desc:年龄
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? Age { get; set; }

        /// <summary>
        /// Desc:性别
        /// Default:
        /// Nullable:True
        /// </summary>           
        public bool? Gender { get; set; }
        /// <summary>
        /// Desc:Sessionid
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Sessionid { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
