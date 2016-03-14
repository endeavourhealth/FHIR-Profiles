using FhirProfilePublisher.Specification;
using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal static class Images
    {
        public static readonly string ImagesResourceLocation = typeof(Images).Assembly.GetName().Name + ".WebContent.Images.";

        public const string EndeavourLogo = "endeavourlogo.png";
        public const string FhirLogo = "fhirlogo.png";
        public const string IconChoice = "icon_choice.gif";
        public const string IconDatatype = "icon_datatype.gif";
        public const string IconElement = "icon_element.gif";
        public const string IconExtensionComplex = "icon_extension_complex.png";
        public const string IconExtensionSimple = "icon_extension_simple.png";
        public const string IconPrimitive = "icon_primitive.png";
        public const string IconProfile = "icon_profile.png";
        public const string IconReference = "icon_reference.png";
        public const string IconResource = "icon_resource.png";
        public const string IconReuse = "icon_reuse.png";
        public const string IconSlice = "icon_slice.png";
        public const string IconTreeVJoin = "tbl_vjoin.png";
        public const string IconTreeVJoinEnd = "tbl_vjoin_end.png";
        public const string IconTreeVLine = "tbl_vline.png";
        public const string IconTreeSpacer = "tbl_spacer.png";
        public const string IconTreeSpacerWide = "tbl_spacerwide.png";
        public const string IconTreeBlank = "tbl_blank.png";
        public const string IconBulletBlack = "bullet_black.png";
        public const string IconBulletGreen = "bullet_green.png";
        public const string IconBulletOrange = "bullet_orange.png";
        public const string IconBlank = "blank.png";

        private static Dictionary<string, string> ImageTitles = new Dictionary<string, string>()
        {
            { IconChoice, "Choice of types"},
            { IconDatatype, "Data type" },
            { IconElement, "Element" },
            { IconExtensionComplex, "Complex extension" },
            { IconExtensionSimple, "Simple extension" },
            { IconPrimitive, "Primitive data type" },
            { IconProfile, "Profile" },
            { IconReference, "Reference" },
            { IconResource, "Resource" },
            { IconReuse, "Reference to another element" },
            { IconSlice, "Slice" }
        };

        public static void WriteImagesToDisk(OutputPaths outputPaths)
        {
            string[] imageNames = GetImageNames();
            
            foreach (string imageName in imageNames)
                ResourceHelper.WriteResourceToDisk(ImagesResourceLocation + imageName, outputPaths.GetOutputPath(OutputFileType.Image, imageName));
        }

        private static string[] GetImageNames()
        {
            return typeof(Images)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && typeof(string).Equals(fi.FieldType))
                .Select(t => (string)t.GetValue(null))
                .ToArray();
        }

        public static string GetImageTitle(string imageName)
        {
            if (ImageTitles.ContainsKey(imageName))
                return ImageTitles[imageName];

            return null;
        }

        public static string GetBackgroundHierarchyImageFilename(bool[] indents, bool hasChildren)
        {
            return "tbl_bck"
                + string.Join(string.Empty, indents.Select(t => t ? "0" : "1"))
                + (hasChildren ? "1" : "0")
                + ".png";
        }

        public static string GetImageName(SDTreeNode treeNode)
        {
            SDNodeType nodeType = treeNode.GetNodeType();

            switch (nodeType)
            {
                case SDNodeType.Resource: return Images.IconResource;
                case SDNodeType.PrimitiveType: return Images.IconPrimitive;
                case SDNodeType.ComplexType: return Images.IconDatatype;
                case SDNodeType.Reference: return Images.IconReference;
                case SDNodeType.Extension: return Images.IconExtensionSimple;
                case SDNodeType.SetupSlice: return Images.IconSlice;
                case SDNodeType.Element: return Images.IconElement;
                case SDNodeType.Choice: return Images.IconChoice;
                case SDNodeType.Unknown: return Images.IconBlank;
                default: throw new NotSupportedException("SDNodeType");
            }
        }

        public static string GenerateBackgroundHierarchyImage(bool[] indents, bool hasChildren, OutputPaths outputPaths)
        {
            string imageName = GetBackgroundHierarchyImageFilename(indents, hasChildren);

            string filePath = outputPaths.GetOutputPath(OutputFileType.Image, imageName);
            
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                using (Bitmap bi = new Bitmap(400, 2))
                {
                    using (Graphics graphics = Graphics.FromImage(bi))
                    {
                        using (Pen pen = new Pen(Color.White))
                        {
                            graphics.DrawRectangle(pen, 0, 0, 400, 2);

                            for (int i = 0; i < indents.Length; i++)
                                if (!indents[i])
                                    bi.SetPixel(12 + (i * 16), 0, Color.Black);

                            if (hasChildren)
                                bi.SetPixel(12 + (indents.Length * 16), 0, Color.Black);

                            bi.Save(stream, ImageFormat.Png);
                        }
                    }
                }
            }

            return imageName;
        }
    }
}
