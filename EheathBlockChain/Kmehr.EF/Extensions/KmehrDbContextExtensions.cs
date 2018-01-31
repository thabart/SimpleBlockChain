using Kmehr.EF.Models;
using Kmehr.EF.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Kmehr.EF.Extensions
{
    internal static class KmehrDbContextExtensions
    {
        public static void EnsureSeedData(this KmehrDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            AddLanguages(context);
            AddHealthCareTypes(context);
            context.SaveChanges();
        }

        private static void AddLanguages(KmehrDbContext context)
        {
            if (!context.Languages.Any())
            {
                context.Languages.AddRange(new[]
                {
                    new Language
                    {
                        Id = "en"
                    },
                    new Language
                    {
                        Id = "fr"
                    },
                    new Language
                    {
                        Id = "nl"
                    },
                    new Language
                    {
                        Id = "de"
                    }
                });
            }
        }

        private static void AddHealthCareTypes(KmehrDbContext context)
        {
            if (!context.HealthCarePartyTypes.Any())
            {
                var assembly = Assembly.GetExecutingAssembly();
                var names = assembly.GetManifestResourceNames();
                CdHcParty result = null;
                using (var stream = assembly.GetManifestResourceStream("Kmehr.EF.Resources.cd-hcparty.xml"))
                {
                    var serializer = new XmlSerializer(typeof(CdHcParty));
                    using (var reader = new StreamReader(stream))
                    {
                        var xml = reader.ReadToEnd();
                        using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                        {
                            result = (CdHcParty)serializer.Deserialize(memStream);
                        }
                    }
                }

                foreach(var value in result.Values)
                {
                    var record = new HealthCarePartyType
                    {
                        Code = value.Code
                    };
                    var descriptions = new List<Translation>();
                    foreach (var v in value.Descriptions)
                    {
                        descriptions.Add(new Translation
                        {
                            LanguageId = v.Language,
                            HealthCarePartyTypeId = value.Code,
                            Value = v.Value
                        });
                    }

                    record.Translations = descriptions;
                    context.HealthCarePartyTypes.Add(record);
                }
            }
        }
    }
}
