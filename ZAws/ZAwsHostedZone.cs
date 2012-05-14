///////////////////////////////////////////////////////////////////////////////
//   Copyright 2012 Z-Ware Ltd.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Amazon.Route53.Model;
using System.Threading;
using ZAws.Console;

namespace ZAws
{
    class ZAwsHostedZone : ZAwsObject
    {
        public HostedZone ResponseData { get; private set; }

        public ZAwsHostedZone(ZAwsEc2Controller controller, HostedZone res)
            : base(controller)
        {
            Update(res);
            RecordsAvailable = false;
        }

        public override string Name
        {
            get
            {
                return ResponseData.Name;
            }
        }

        public override string Id
        {
            get
            {
                return ResponseData.Id;
            }
        }

        protected override bool DoUpdate(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(HostedZone), "Wrong data passed to the object for update.");
            ResponseData = (HostedZone)responseData;
            return true;
        }

        internal override bool EqualsData(object responseData)
        {
            Debug.Assert(responseData.GetType() == typeof(HostedZone), "Wrong data passed to the object for update.");
            return string.Equals(Id, ((HostedZone)responseData).Id);
        }

        protected override void DoDeleteObject()
        {
            DeleteHostedZoneResponse resp = myController.route53.DeleteHostedZone(
                                                new DeleteHostedZoneRequest()
                                                                    .WithId(this.ResponseData.Id));
        }

        internal List<ResourceRecordSet> currentRecordSet = new List<ResourceRecordSet>();


        public bool RecordsAvailable { get; private set; }
        internal void UpdateInfo()
        {
            ListResourceRecordSetsResponse resp = myController.route53.ListResourceRecordSets(new ListResourceRecordSetsRequest()
                                                                .WithHostedZoneId(this.ResponseData.Id));

            //Check if there is a change
            bool Change = (currentRecordSet.Count != resp.ListResourceRecordSetsResult.ResourceRecordSets.Count);
            for (int i = 0; (!Change) && i < currentRecordSet.Count && i < resp.ListResourceRecordSetsResult.ResourceRecordSets.Count; i++)
            {
                Change =
                    currentRecordSet[i].Name != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].Name
                    || currentRecordSet[i].SetIdentifier != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].SetIdentifier
                    || currentRecordSet[i].TTL != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].TTL
                    || currentRecordSet[i].Type != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].Type
                    || currentRecordSet[i].Weight != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].Weight
                    || currentRecordSet[i].ResourceRecords.Count != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].ResourceRecords.Count
                    ;

                if (resp.ListResourceRecordSetsResult.ResourceRecordSets[i].AliasTarget == null)
                {
                    Change |= currentRecordSet[i].AliasTarget != null;
                }
                else
                {
                    Change |= currentRecordSet[i].AliasTarget == null;
                    Change |= currentRecordSet[i].AliasTarget.DNSName != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].AliasTarget.DNSName;
                    Change |= currentRecordSet[i].AliasTarget.HostedZoneId != resp.ListResourceRecordSetsResult.ResourceRecordSets[i].AliasTarget.HostedZoneId;
                }

                if(Change) break;

                foreach(var rr in currentRecordSet[i].ResourceRecords)
                {
                    bool found = false;
                    foreach (var r in resp.ListResourceRecordSetsResult.ResourceRecordSets[i].ResourceRecords)
                    {
                        if (rr.Value == r.Value)
                        {
                            found = true;
                            break;
                        }
                    }
                    Change |= !found;
                }
            }


            if (Change)
            {
                RecordsAvailable = true;
                currentRecordSet = resp.ListResourceRecordSetsResult.ResourceRecordSets;
                TriggerStatusChanged();
            }            
        }

        public string[] Targets
        {
            get
            {
                List<string> targetList = new List<string>();
                foreach (var r in currentRecordSet)
                {
                    if (r.Type == "A")
                    {
                        string newTarget = (r.ResourceRecords != null && r.ResourceRecords.Count == 1) ? 
                                                r.ResourceRecords[0].Value : "";
                        ZAwsElasticIp ip = null;
                        try
                        {
                            ip = myController.GetElasticIp(newTarget);
                            if (ip.Associated)
                            {
                                newTarget = ip.AssociatedEc2.Name;
                            }
                            else
                            {
                                newTarget += " (el.ip)";
                            }
                        }
                        catch { }

                        bool found = false;
                        foreach (var s in targetList)
                        {
                            if (s == newTarget)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            targetList.Add(newTarget);
                        }
                    }
                }
                return targetList.ToArray();
            }
        }

        internal bool AddRecord(ResourceRecordSet s)
        {
            try
            {
                ChangeResourceRecordSetsResponse resp =
                myController.route53.ChangeResourceRecordSets(new ChangeResourceRecordSetsRequest()
                                                                    .WithHostedZoneId(this.ResponseData.Id)
                                                                    .WithChangeBatch(new ChangeBatch()
                                                                                        .WithChanges(
                                                                                            new Change()
                                                                                                .WithAction("CREATE")
                                                                                                .WithResourceRecordSet(s))));
                Thread.Sleep(2000);
                UpdateInfo();
                return true;
            }
            catch (Exception ex)
            {
                Program.TraceLine("Failed adding a recordset, reason: ", ex);
                return false;
            }
        }

        internal void DeleteRecord(ResourceRecordSet s)
        {
            ChangeResourceRecordSetsResponse resp = 
            myController.route53.ChangeResourceRecordSets(new ChangeResourceRecordSetsRequest()
                                                                .WithHostedZoneId(this.ResponseData.Id)
                                                                .WithChangeBatch(new ChangeBatch()
                                                                                    .WithChanges(
                                                                                        new Change()
                                                                                            .WithAction("DELETE")
                                                                                            .WithResourceRecordSet(s))));
            Thread.Sleep(2000);
            UpdateInfo();
        }
    }
}
