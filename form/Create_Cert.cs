using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool.form
{
    public partial class Create_Cert : Form
    {
        public Create_Cert()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;

                if (foldPath.Contains(" "))
                {
                    MessageBox.Show("路径不能包括空格，请重新选择");
                    return;
                }
                selectedPath.Text = foldPath + "\\";
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var path = selectedPath.Text;
            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("路径不能为空");
                return;
            }
            this.Cursor = Cursors.WaitCursor;

            try
            {
                //var generator = new CertificateGenerator();
                GenerateAndExportCertificates(
                    certificateName: "MyCertificate",
                    pfxPassword: "YourSecurePassword",
                    addToTrustedStore: false, // 设置为true将证书添加到受信任存储
                    outputDirectory: @"C:\Certificates");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成证书时出错: {ex.Message}");
            }
        }

        // 证书有效期（年）
        private const int CertificateValidYears = 1;

        // 创建证书并导出相关文件
        public void GenerateAndExportCertificates(
            string certificateName,
            string pfxPassword,
            bool addToTrustedStore = false,
            string outputDirectory = "")
        {
            // 确保输出目录存在
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 构建完整路径
            string GetFilePath(string extension) =>
                Path.Combine(outputDirectory, $"{certificateName}{extension}");

            // 创建自签名证书
            var certificate = CreateSelfSignedCertificate(certificateName);

            // 导出PFX文件（包含私钥）
            ExportPfxFile(certificate, pfxPassword, GetFilePath(".pfx"));

            // 导出公钥证书(.crt)
            ExportCertificateFile(certificate, GetFilePath(".crt"));

            // 导出私钥(.key)
            ExportPrivateKeyFile(certificate, GetFilePath(".key"));

            // 添加到受信任存储
            if (addToTrustedStore)
            {
                AddCertificateToTrustedStore(certificate);
            }

            Console.WriteLine("证书生成和导出完成!");
        }

        // 创建自签名证书
        private X509Certificate2 CreateSelfSignedCertificate(string certificateName)
        {
            using (var rsa = RSA.Create(2048))
            {
                var certificateRequest = new CertificateRequest(
                    new X500DistinguishedName($"CN={certificateName}"),
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                // 设置证书基本约束
                certificateRequest.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(true, false, 0, true));

                // 设置密钥用途
                certificateRequest.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.KeyEncipherment |
                        X509KeyUsageFlags.DigitalSignature |
                        X509KeyUsageFlags.KeyCertSign, true));

                // 设置增强密钥用途
                certificateRequest.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection
                        {
                        new Oid("1.3.6.1.5.5.7.3.1"), // 服务器身份验证
                        new Oid("1.3.6.1.5.5.7.3.2")  // 客户端身份验证
                        }, true));

                // 设置有效期
                var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
                var notAfter = DateTimeOffset.UtcNow.AddYears(CertificateValidYears);

                // 创建自签名证书
                var certificate = certificateRequest.CreateSelfSigned(notBefore, notAfter);

                // 将证书转换为包含私钥的版本
                return new X509Certificate2(
                    certificate.Export(X509ContentType.Pfx),
                    (string)null,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }
        }

        // 导出PFX文件
        private void ExportPfxFile(X509Certificate2 certificate, string password, string filePath)
        {
            var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
            File.WriteAllBytes(filePath, pfxBytes);
            Console.WriteLine($"PFX文件已导出至: {filePath}");
        }

        // 导出证书文件(.crt)
        private void ExportCertificateFile(X509Certificate2 certificate, string filePath)
        {
            var certBytes = certificate.Export(X509ContentType.Cert);

            // 转换为PEM格式
            var pemContent = "-----BEGIN CERTIFICATE-----\n" +
                             Convert.ToBase64String(certBytes, Base64FormattingOptions.InsertLineBreaks) +
                             "\n-----END CERTIFICATE-----";

            File.WriteAllText(filePath, pemContent);
            Console.WriteLine($"证书文件(.crt)已导出至: {filePath}");
        }

        // 导出私钥文件(.key)
        private void ExportPrivateKeyFile(X509Certificate2 certificate, string filePath)
        {
            if (!certificate.HasPrivateKey)
            {
                throw new InvalidOperationException("证书不包含私钥");
            }

            using (var rsa = certificate.GetRSAPrivateKey())
            {
                if (rsa == null)
                {
                    throw new InvalidOperationException("无法获取RSA私钥");
                }

                // 获取私钥参数
                var rsaParameters = rsa.ExportParameters(true);

                // 创建PKCS#8私钥结构
                using (var privateKeyWriter = new MemoryStream())
                {
                    using (var writer = new BinaryWriter(privateKeyWriter))
                    {
                        // 写入PKCS#8 PrivateKeyInfo头部
                        writer.Write(new byte[] { 0x30, 0x82 }); // SEQUENCE + 长度(2字节)
                        writer.Write(0); // 占位符，稍后填充总长度

                        // 版本
                        writer.Write(new byte[] { 0x02, 0x01, 0x00 }); // INTEGER 0

                        // 算法标识符
                        writer.Write(new byte[] { 0x30, 0x0D }); // SEQUENCE + 长度
                        writer.Write(new byte[] { 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01 }); // OID rsaEncryption
                        writer.Write(new byte[] { 0x05, 0x00 }); // NULL

                        // 私钥
                        writer.Write(new byte[] { 0x04, 0x82 }); // OCTET STRING + 长度(2字节)
                        writer.Write(0); // 占位符，稍后填充私钥长度

                        // 内部PKCS#1 RSAPrivateKey结构
                        writer.Write(new byte[] { 0x30, 0x82 }); // SEQUENCE + 长度(2字节)
                        writer.Write(0); // 占位符，稍后填充内部结构长度

                        // 版本
                        writer.Write(new byte[] { 0x02, 0x01, 0x00 }); // INTEGER 0

                        // 模数(n)
                        WriteInteger(writer, rsaParameters.Modulus);

                        // 公共指数(e)
                        WriteInteger(writer, rsaParameters.Exponent);

                        // 私有指数(d)
                        WriteInteger(writer, rsaParameters.D);

                        // 素数p
                        WriteInteger(writer, rsaParameters.P);

                        // 素数q
                        WriteInteger(writer, rsaParameters.Q);

                        // 指数1 (d mod (p-1))
                        WriteInteger(writer, rsaParameters.DP);

                        // 指数2 (d mod (q-1))
                        WriteInteger(writer, rsaParameters.DQ);

                        // 系数 (q^-1 mod p)
                        WriteInteger(writer, rsaParameters.InverseQ);

                        // 计算并填充长度
                        var innerLength = (int)writer.BaseStream.Position - 12; // 减去内部SEQUENCE标记和长度占位符
                        writer.BaseStream.Seek(24, SeekOrigin.Begin);
                        writer.Write(innerLength >> 8);
                        writer.Write(innerLength & 0xFF);

                        var privateKeyLength = (int)writer.BaseStream.Position - 20; // 减去OCTET STRING标记和长度占位符
                        writer.BaseStream.Seek(14, SeekOrigin.Begin);
                        writer.Write(privateKeyLength >> 8);
                        writer.Write(privateKeyLength & 0xFF);

                        var totalLength = (int)writer.BaseStream.Position - 4; // 减去SEQUENCE标记和长度占位符
                        writer.BaseStream.Seek(2, SeekOrigin.Begin);
                        writer.Write(totalLength >> 8);
                        writer.Write(totalLength & 0xFF);
                    }

                    // 转换为PEM格式
                    var pemContent = "-----BEGIN PRIVATE KEY-----\n" +
                                     Convert.ToBase64String(privateKeyWriter.ToArray(), Base64FormattingOptions.InsertLineBreaks) +
                                     "\n-----END PRIVATE KEY-----";

                    File.WriteAllText(filePath, pemContent);
                    Console.WriteLine($"私钥文件(.key)已导出至: {filePath}");
                }
            }
        }

        // 添加证书到受信任的根证书颁发机构存储
        private void AddCertificateToTrustedStore(X509Certificate2 certificate)
        {
            try
            {
                using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    // 打开存储以进行读写
                    store.Open(OpenFlags.ReadWrite);

                    // 检查证书是否已存在
                    var certificates = store.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        certificate.Thumbprint,
                        false);

                    if (certificates.Count == 0)
                    {
                        // 添加证书到存储
                        store.Add(certificate);
                        Console.WriteLine($"证书已添加到受信任的根证书颁发机构存储 (指纹: {certificate.Thumbprint})");
                    }
                    else
                    {
                        Console.WriteLine($"证书已存在于受信任的根证书颁发机构存储中 (指纹: {certificate.Thumbprint})");
                    }

                    // 关闭存储
                    store.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加证书到信任存储时出错: {ex.Message}");
                Console.WriteLine("提示: 此操作需要管理员权限。请确保以管理员身份运行应用程序。");
            }
        }

        // 辅助方法：写入ASN.1 INTEGER类型
        private void WriteInteger(BinaryWriter writer, byte[] value)
        {
            // 移除前导零
            int offset = 0;
            while (offset < value.Length - 1 && value[offset] == 0)
            {
                offset++;
            }

            // 检查是否需要添加前导零以确保正数
            bool needsLeadingZero = (value[offset] & 0x80) != 0;

            int length = value.Length - offset;
            if (needsLeadingZero)
            {
                length++;
            }

            writer.Write((byte)0x02); // INTEGER标记
            writer.Write((byte)length);

            if (needsLeadingZero)
            {
                writer.Write((byte)0);
            }

            writer.Write(value, offset, value.Length - offset);
        }
    }
}
