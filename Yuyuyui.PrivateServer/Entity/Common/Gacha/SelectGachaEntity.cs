﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.DataModel;

namespace Yuyuyui.PrivateServer
{
    public class SelectGachaEntity : BaseEntity<SelectGachaEntity>
    {
        public SelectGachaEntity(
            Uri requestUri,
            string httpMethod,
            Dictionary<string, string> requestHeaders,
            byte[] requestBody,
            RouteConfig config)
            : base(requestUri, httpMethod, requestHeaders, requestBody, config)
        {
        }

        protected override Task ProcessRequest()
        {
            var player = GetPlayerFromCookies();

            using var gachasDb = new GachasContext();
            var selectGachas = GachaEntity.GetCurrentActiveGachas(gachasDb, player)
                .Where(g => g.SelectGacha == "1");

            Response responseObj = new()
            {
                contents = selectGachas.Select(g => new Response.SelectGachaInfo
                {
                    gacha_id = g.Id, // TODO: This might be the step up group
                    cards = GetSelectGachaContents(gachasDb, g),
                    selected_cards = GetPlayerSelectedCards(player, gachasDb, g)
                }).ToList()
            };

            responseBody = Serialize(responseObj);
            SetBasicResponseHeaders();

            return Task.CompletedTask;
        }

        private List<Response.SelectContent> GetSelectGachaContents(GachasContext gachasDb, Gacha gacha)
        {
            var gachaBoxId = gacha.StepupGroup ?? gacha.Id;
            return gachasDb.GachaContents
                .Where(c => c.GachaBoxId == gachaBoxId)
                .Where(c => c.SelectId != null)
                .Select(c => new Response.SelectContent
                {
                    content_id = c.ContentId,
                    select_id = c.SelectId ?? -1 // made sure to be not null
                })
                .Distinct()
                .ToList();
        }

        private List<Response.SelectedContent> GetPlayerSelectedCards(PlayerProfile player, GachasContext gachasDb, Gacha gacha)
        {
            var gachaBoxId = gacha.StepupGroup ?? gacha.Id;
            bool hasSelected = player.gachaSelections.TryGetValue(gachaBoxId, out var selected);

            if (!hasSelected) return new List<Response.SelectedContent>();

            return selected!.Select(s => new Response.SelectedContent
            {
                content_id = gachasDb.GachaContents.First(g => g.GachaBoxId == gachaBoxId && g.SelectId == s).ContentId
            }).ToList();
        }

        public class Response
        {
            public IList<SelectGachaInfo> contents { get; set; } = new List<SelectGachaInfo>();

            public class SelectGachaInfo
            {
                public long gacha_id { get; set; }
                public IList<SelectContent> cards { get; set; } = new List<SelectContent>();
                public IList<SelectedContent> selected_cards = new List<SelectedContent>();
            }

            public class SelectContent : IEquatable<SelectContent>
            {
                public long content_id { get; set; }
                public int select_id { get; set; }

                public bool Equals(SelectContent? other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return content_id == other.content_id && select_id == other.select_id;
                }

                public override bool Equals(object? obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != this.GetType()) return false;
                    return Equals((SelectContent)obj);
                }

                public override int GetHashCode()
                {
                    return HashCode.Combine(content_id, select_id);
                }
            }

            public class SelectedContent
            {
                public long content_id { get; set; }
            }
        }
    }
}