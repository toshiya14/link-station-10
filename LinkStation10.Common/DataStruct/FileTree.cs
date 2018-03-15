using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMEGo.Sunflower.LinkStation10.Common
{
    public enum ItemType
    {
        Context,
        EndPoint
    }
    public class MenuItem
    {
        public Image Icon;
        public string Name;
        public ItemType Type;
        public string StartCommand;
    }
    public class FileTree
    {
        private static Image DirIcon {
            get => Image.FromFile(@"../../../Assets/folder.png");
        }

        public Node<MenuItem> BuildFromRootFolder(string rootpath)
        {
            if (!isFolder(rootpath))
            {
                throw new DirectoryNotFoundException("Could not find the directory: " + rootpath);
            }
            var root = new Node<MenuItem>("<root>", new MenuItem());
            FillTree(root, rootpath);
            return root;
        }
        private static void FillTree(Node<MenuItem> node, string refpath)
        {
            if (isFolder(refpath))
            {
                var dirinfo = new DirectoryInfo(refpath);
                foreach (var dir in dirinfo.GetDirectories())
                {
                    if (dir.Name.StartsWith(".") || dir.Attributes.HasFlag(FileAttributes.Hidden) || dir.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }
                    if (node[dir.Name] == null)
                    {
                        node[dir.Name] = new Node<MenuItem>(dir.Name,
                            new MenuItem
                            {
                                Icon = GetDirIcon(dir.FullName),
                                Name = GetDirName(dir.FullName),
                                Type = ItemType.Context
                            });
                    }
                    else
                    {
                        node[dir.Name].Value = new MenuItem
                        {
                            Icon = GetDirIcon(dir.FullName),
                            Name = GetDirName(dir.FullName),
                            Type = ItemType.Context
                        };
                    }
                    FillTree(node[dir.Name], dir.FullName);
                }

                foreach (var file in dirinfo.GetFiles())
                {
                    if (file.Name.StartsWith(".") || file.Attributes.HasFlag(FileAttributes.Hidden) || file.Attributes.HasFlag(FileAttributes.System))
                    {
                        continue;
                    }
                    if (node[file.Name] == null)
                    {
                        node[file.Name] = new Node<MenuItem>(file.Name,
                            new MenuItem
                            {
                                Icon = GetFileIcon(file.FullName),
                                Name = GetFileName(file.FullName),
                                Type = ItemType.EndPoint,
                                StartCommand = GetFileCommand(file.FullName)
                            });
                    }
                    else
                    {
                        node[file.Name].Value = new MenuItem
                        {
                            Icon = GetFileIcon(file.FullName),
                            Name = GetFileName(file.FullName),
                            Type = ItemType.EndPoint,
                            StartCommand = GetFileCommand(file.FullName)
                        };
                    }
                }
            }
        }
        private static bool isFolder(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static Image GetFileIcon(string filename)
        {
            var fileinfo = new FileInfo(filename);
            var ext = fileinfo.Extension;
            return IconHandler.IconHandler.IconFromExtension(ext, IconHandler.IconSize.Large).ToBitmap();
        }
        private static string GetFileName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }
        private static Image GetDirIcon(string filename)
        {
            var cached = CachedDesktopIniLoader.GetIconPathConfig(filename);
            var index = CachedDesktopIniLoader.GetIconIndexConfig(filename);
            if (string.IsNullOrWhiteSpace(cached))
            {
                return DirIcon;
            }
            else
            {
                var path = string.Empty;
                if (Path.IsPathRooted(cached))
                {
                    path = Path.GetFullPath(cached);
                }
                else {
                    if (cached.Contains("%"))
                    {
                        path = Environment.ExpandEnvironmentVariables(cached);
                    }
                    else {
                        path = Path.Combine(filename, cached);
                    }
                }
                var icon = IconHandler.IconHandler.IconFromFile(path, IconHandler.IconSize.Large, index);
                return icon.ToBitmap();
            }
        }
        private static string GetDirName(string filename)
        {
            var cached = CachedDesktopIniLoader.GetLocalizedResourceNameConfig(filename);
            if (string.IsNullOrWhiteSpace(cached))
            {
                var fi = new DirectoryInfo(filename);
                return fi.Name;
            }
            else
            {
                return cached;
            }
        }
        private static string GetFileCommand(string filename)
        {
            return filename;
        }
    }
}
