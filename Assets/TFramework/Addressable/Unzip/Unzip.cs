using System.IO;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TFramework.Addressable
{
    public static class Unzip
    {
        /// <summary>
        /// Unity 万能获取【程序根目录】的方法 (编辑器/Windows打包后通用)
        /// 打包后 = EXE所在文件夹根目录；编辑器中 = Unity工程根目录
        /// </summary>
        public static string GetAppRootPath()
        {
            string rootPath = Application.dataPath;
            // Windows打包后，路径是 xxx_Data，返回上一级就是EXE根目录
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                rootPath = Directory.GetParent(rootPath)?.FullName;
            }
            return rootPath;
        }

        public static async void UnZipToRoot(string addressablePath,bool overwriteFiles = true)
        {
            if(string.IsNullOrEmpty(addressablePath))
                return;
            Debug.Log($"开始加载Zip包，资源Key：{addressablePath}");

            // 1. Addressables异步加载Zip二进制文件 → 转为TextAsset
            var loadHandle =  Addressables.LoadAssetAsync<TextAsset>(addressablePath);
            await loadHandle.Task;

            // 2. 加载结果判断
            if (loadHandle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Zip包加载失败！错误：{loadHandle.OperationException?.Message}");
                Addressables.Release(loadHandle); // 释放资源，防止内存泄漏
                return;
            }

            // 3. 获取Zip包的二进制字节数组（核心数据）
            TextAsset zipTextAsset = loadHandle.Result;
            byte[] zipBytes = zipTextAsset.bytes;
            if (zipBytes == null || zipBytes.Length == 0)
            {
                Debug.LogError("加载到的Zip包为空，字节数组长度为0");
                Addressables.Release(loadHandle);
                return;
            }
            Debug.Log($"Zip包加载成功，字节大小：{zipBytes.Length / 1024} KB");

            var rootPath = GetAppRootPath();
            // 4. 调用解压方法：内存流解压（核心！不需要写入本地文件）
            bool isExtractSuccess = await ExtractZipFromBytes(zipBytes, rootPath,overwriteFiles);

            // 5. 解压结果回调
            if (isExtractSuccess)
            {
                Debug.Log($"✅ 解压成功！所有文件已写入：{rootPath}");
            }
            else
            {
                Debug.LogError("❌ 解压失败！");
            }

            // 6. 释放资源（Addressables必须手动释放，否则内存泄漏）
            Addressables.Release(loadHandle);
        }
        
        /// <summary>
        /// 核心解压方法：通过字节数组 内存流解压 Zip/7z 通用
        /// 无本地文件IO，效率极高，支持Zip/7z自动识别，完美兼容中文路径/文件名
        /// </summary>
        /// <param name="archiveBytes">Zip/7z的二进制字节数组</param>
        /// <param name="extractPath">解压目标路径</param>
        /// <returns>是否解压成功</returns>
        public static async Task<bool> ExtractZipFromBytes(byte[] archiveBytes, string extractPath, bool overwriteFiles = true)
        {
            try
            {
                // 自动创建解压目录（不存在则创建）
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                // 内存中创建流，无需写入本地文件，直接解压（核心优化）
                using (MemoryStream ms = new MemoryStream(archiveBytes))
                {
                    // SharpCompress自动识别压缩包格式（Zip/7z都支持）
                    using (var archive = ArchiveFactory.Open(ms))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            // 跳过空文件夹，只解压文件
                            if (!entry.IsDirectory)
                            {
                                // 写入解压目录，设置覆盖已有文件
                                entry.WriteToDirectory(extractPath, new ExtractionOptions()
                                {
                                    ExtractFullPath = true, // 保留压缩包内的目录层级
                                    Overwrite = overwriteFiles        // 覆盖已存在的文件
                                });
                            }
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"解压异常：{ex.Message} \n {ex.StackTrace}");
                return await Task.FromResult(false);
            }
        }
        /// <summary>
        /// 通用解压方法：同时支持 ZIP 格式 和 7Z 格式（自动识别）
        /// </summary>
        /// <param name="archiveFilePath">压缩包完整路径（zip/7z均可）</param>
        /// <param name="extractPath">解压目标路径</param>
        /// <param name="overwriteFiles">是否覆盖已存在文件</param>
        static void ExtractArchiveFile(string archiveFilePath, string extractPath, bool overwriteFiles = true)
        {
            if(string.IsNullOrEmpty(archiveFilePath) || string.IsNullOrEmpty(extractPath))
                return;
            // 校验压缩包文件是否存在
            if (!File.Exists(archiveFilePath))
            {
                throw new FileNotFoundException("指定的压缩包文件不存在", archiveFilePath);
            }

            // 自动创建解压目录
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            // 核心逻辑：SharpCompress自动识别压缩包格式，统一解压逻辑
            using (var archive = ArchiveFactory.Open(archiveFilePath))
            {
                foreach (var entry in archive.Entries)
                {
                    // 跳过空目录
                    if (!entry.IsDirectory)
                    {
                        // 解压文件，设置覆盖规则
                        entry.WriteToDirectory(extractPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = overwriteFiles
                        });
                    }
                }
            }
        }
    }
}

