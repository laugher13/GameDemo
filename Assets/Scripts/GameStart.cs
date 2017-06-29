using UnityEngine;
using System.Collections;
using System.IO;

public class GameStart : MonoBehaviour {

    private LuaScriptMgr luaMgr;
    private string message;

	void Start () {
        CheckExtractResource();
	}
    void Update()
    {
        if (luaMgr != null)
        {
            luaMgr.Update();
        }  
    }

    void LateUpdate()
    {
        if (luaMgr != null)
        {
            luaMgr.LateUpate();
        }
    }

    void FixedUpdate()
    {
        if (luaMgr != null)
        {
            luaMgr.FixedUpdate();
        }
    }

	
    /// <summary>
    /// 释放资源
    /// </summary>
    public void CheckExtractResource()
    {
        string path = Application.persistentDataPath + "/Game/";
        bool isExists = Directory.Exists(path) &&
          Directory.Exists(path + "lua/") && File.Exists(path + "files.txt");
        if (isExists)
        {
            luaMgr = new LuaScriptMgr();
            luaMgr.DoFile("Game/Login");
            luaMgr.Start();
            luaMgr.CallLuaFunction("GetUI", this.gameObject);
            print("-----------文件已经解压过了-----------");
            return;   //文件已经解压过了，自己可添加检查文件列表逻辑
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    /// <summary>
    /// 游戏开始前解压ToLua包
    /// </summary>
    /// <returns></returns>
    IEnumerator OnExtractResource()
    {
        string dataPath = Application.persistentDataPath + "/Game/";
        string resPath = Application.streamingAssetsPath + "/";

        if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
        Directory.CreateDirectory(dataPath);

        string infile = resPath + "files.txt";
        string outfile = dataPath + "files.txt";
        if (File.Exists(outfile)) File.Delete(outfile);

        message = "正在解包文件:>files.txt";
        print("-----------正在解包文件-----------");
        Debug.Log(infile);
        Debug.Log(outfile);    
        if (Application.platform == RuntimePlatform.Android)
        {
            WWW www = new WWW(infile);
            yield return www;

            if (www.isDone)
            {
                File.WriteAllBytes(outfile, www.bytes);
            }
            yield return 0;
        }
        else File.Copy(infile, outfile, true);
        yield return new WaitForEndOfFrame();

        //释放所有文件到数据目录
        string[] files = File.ReadAllLines(outfile);
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            infile = resPath + fs[0];  //
            outfile = dataPath + fs[0];

            message = "正在解包文件:>" + fs[0];
            Debug.Log("正在解包文件:>" + infile);

            string dir = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (Application.platform == RuntimePlatform.Android)
            {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone)
                {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            }
            else
            {
                if (File.Exists(outfile))
                {
                    File.Delete(outfile);
                }
                File.Copy(infile, outfile, true);
            }
            yield return new WaitForEndOfFrame();
        }
        message = "解包完成!!!";
        Debug.Log(message);
        yield return new WaitForSeconds(0.1f);
     
        message = string.Empty;
        //释放完成，开始启动更新资源
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 30, 960, 50), message);
    }
}
