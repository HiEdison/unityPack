using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.ScrollRect
{
    [System.Serializable]
    public class DataModel
    {
        public string msg;
        public DataModel(Vector2 v)
        {
            if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                msg = GetRandomString((int)UnityEngine.Random.Range(v.x, v.y), true, true, true, true, "2112");
            else
            {
                msg = "";
            }
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            System.Random r = new System.Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }

    public class TestList : ScrollCtrlBase<DataModel>
    {
        public int count = 1;
        public Vector2 range = new Vector2(50, 200);
        IEnumerator Start()
        {
            srollrect = gameObject.GetComponent<CustomScroll>();
            while (list.Count < count)
            {
                for (int i = 0; i < 1000; i++)
                {
                    list.Add(new DataModel(range));
                    if (list.Count == count)
                    {
                        break;
                    }
                }
                yield return 0;
            }
            srollrect.totalCount = count;
            srollrect.UpdateCell = OnUpdate;
            srollrect.RefillCells();
        }

        private void OnUpdate(Transform arg1, int arg2)
        {
            arg1.GetComponent<TestItem>().Ref(list[arg2]);
        }
        public int addcount = 1;

        private void OnGUI()
        {
            if (addcount < 1) addcount = 1;
            addcount = (int)GUILayout.HorizontalSlider(addcount, 1f, 20f);
            GUILayout.Label(addcount.ToString());

            if (GUILayout.Button("insert 0"))
            {
                InsertFirst(Generate());
            }
            if (GUILayout.Button("add"))
            {
                Add(Generate());
            }
            if (GUILayout.Button("insert 0, and goto frist"))
            {
                GotoFirst(Generate());
            }
            if (GUILayout.Button("add and goto last"))
            {
                GotoEnd(Generate());
            }
            if (GUILayout.Button("goto frist"))
            {
                GotoFirst();
            }
            if (GUILayout.Button("goto last"))
            {
                GotoEnd();
            }
        }

        List<DataModel> Generate()
        {
            List<DataModel> ls1 = new List<DataModel>();
            if (addcount <= 0) addcount = 1;
            if (addcount > 0)
            {
                for (int i = 0; i < addcount; i++)
                    ls1.Add(new DataModel(range));
            }
            return ls1;
        }
    }

}
