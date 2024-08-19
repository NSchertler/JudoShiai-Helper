using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Microsoft.Data.Sqlite;
using ReactiveUI;
using Shiai_Helper.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tmds.DBus;

namespace Shiai_Helper
{
    public class ShiaiLoader : ReactiveObject
    {       
        private string? path;
        public string? Path 
        {
            get => path;
            private set
            {
                path = value;
                if(Connection != null)
                {
                    Connection.Dispose();
                    Connection = null;
                }
                this.RaisePropertyChanged(nameof(Path));
            }
        }

        public DbConnection? Connection { get; private set; }

        public Window? Parent { get; set; }

        private Tournament? tournament;
        public Tournament? Tournament 
        {
            get => tournament;
            private set => this.RaiseAndSetIfChanged(ref tournament, value);
        }

        public async void LoadShiai()
        {
            if (Parent == null)
                throw new InvalidOperationException("The loader does not have an associated parent.");

            var shiaiFilter = new FilePickerFileType("Shiai-Turniere");
            shiaiFilter.Patterns = ["*.shi"];
            var results = await Parent.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = false,
                Title = "Wettkampf öffnen",
                FileTypeFilter = [ shiaiFilter ]
            });            

            if (!results.Any())
                return;

            var file = results[0];

            Path = file.Path.LocalPath;
            var t = new Tournament();
            Load(t);
            Tournament = t;            
        }

        public void Load(Tournament tournament)
        {
            if (Connection == null)
            {
                Connection = new SqliteConnection($"Data Source=\"{path}\"");
                Connection.Open();
            }

            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT value FROM info WHERE item='Competition'";
            tournament.Name = command.ExecuteScalar()!.ToString()!;
            
            RetrieveCategories(tournament);
            RetrieveCompetitors(tournament);
        }

        private void RetrieveCompetitors(Tournament tournament)
        {
            if (Connection == null)
                throw new InvalidOperationException("There must be an active connection to retrieve tournament data.");

            tournament.Competitors.Clear();

            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM competitors WHERE deleted & 1 <> 1";

            var categoryRegex = new Regex("^(?<agecat>.+?)?(?<weightcat>[+-].+)?$");

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var comp = new Competitor();
                    comp.Id = reader.GetInt32("index");
                    comp.FirstName = reader.GetString("first");
                    comp.LastName = reader.GetString("last");
                    comp.Club = reader.GetString("club");
                    comp.CoachId = reader.GetString("coachId");

                    var category = reader.GetString("regcategory");
                    var flags = reader.GetInt32("deleted");

                    var parsedCategory = categoryRegex.Match(category);
                    if (parsedCategory != null)
                    {
                        comp.AgeCategory = parsedCategory.Groups["agecat"].Value;
                        comp.WeightCategory = parsedCategory.Groups["weightcat"].Value;
                    }

                    if ((flags & 128) > 0)
                        comp.Gender = Gender.Male;
                    else if ((flags & 256) > 0)
                        comp.Gender = Gender.Female;

                    tournament.Competitors.Add(comp.Id, comp);
                }
            }
        }

        private void RetrieveCategories(Tournament tournament)
        {
            if (Connection == null)
                throw new InvalidOperationException("There must be an active connection to retrieve tournament data.");

            tournament.Categories.Clear();

            var command = Connection.CreateCommand();
            command.CommandText = @"SELECT * FROM categories WHERE deleted=0";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var cat = new Category(reader.GetString("category"), reader.GetInt32("index"));

                    for (int i = 1; i <= 8; ++i)
                    {
                        var posi = reader.GetInt32($"pos{i}");
                        if (posi != 0)
                        {
                            var place = i;
                            if (i == 4)
                                place = 3;
                            else if (i == 6)
                                place = 5;
                            else if (i == 8)
                                place = 7;
                            cat.AddWinner(posi, place);
                        }
                    }

                    tournament.Categories.Add(cat.Id, cat);
                }
            }

            command.CommandText =
                @"SELECT 
                            matches.category, 
                            blue_score+white_score+blue_points+white_points > 0 or blue=1 or white=1 or (w.deleted IS NOT NULL and w.deleted & 3) or (b.deleted IS NOT NULL and b.deleted & 3) as fought, 
                            COUNT(*) as nbr
                        FROM matches 
                        LEFT JOIN competitors b ON blue=b.[index] 
                        LEFT JOIN competitors w ON white=w.[index] 
                        GROUP BY matches.category, fought";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var catId = reader.GetInt32("category");
                    var fought = reader.GetBoolean("fought");
                    var nbr = reader.GetInt32("nbr");

                    var cat = tournament.Categories[catId];
                    if (fought)
                        cat.FoughtMatches = nbr;
                    else
                        cat.UnfoughtMatches = nbr;
                }
            }
        }        
    }
}
