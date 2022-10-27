//#region Copyright
////
//// ©PEER Intellectual Property Inc., 2022
//// 
//// This software contains confidential and trade secret information belonging to
//// PEER Intellectual Property Inc. All rights reserved. 
////
//// No part of this software may be reproduced or transmitted in any form 
//// or by any means, electronic, mechanical, photocopying, recording or 
//// otherwise, without the prior written consent of PEER Intellectual Property Inc.
////
//#endregion
//using Acs.Common.DataType;
//using System;
//using System.Collections.Generic;
//using MQTTnet;

//namespace RITdb.Entity
//{
//    [Serializable]
//    public class LotDataParameterType : DataStructure
//    {
//        private static readonly string[] s_fieldNames = { "Topic", "CorrelationData", "Date", "ResponseTopic", "Payload", "QoS", "Retain", "MetaDataOmap", "ClientID", "UserProperties" };

//        public LotDataParameterType() : base(s_fieldNames)
//        {
//        }

//        public LotDataParameterType(object[] objects) :
//            base(s_fieldNames, objects)
//        {
//            // UserProperties = userProperties;
//        }

//        public string Topic
//        {
//            get
//            {
//                return (string)FieldValues[0];
//            }
//            set
//            {
//                FieldValues[0] = value;
//            }
//        }

//        public byte[] CorrelationData
//        {
//            get
//            {
//                return (byte[])FieldValues[1];
//            }
//            set
//            {
//                FieldValues[1] = value;
//            }
//        }

//        public Acs.Common.DataType.LocalDate Date
//        {
//            get
//            {
//                return (Acs.Common.DataType.LocalDate)FieldValues[2];
//            }
//            set
//            {
//                FieldValues[2] = value;
//            }
//        }
//        public string ResponseTopic

//        {
//            get
//            {
//                return (string)FieldValues[3];
//            }
//            set
//            {
//                FieldValues[3] = value;
//            }
//        }
//        public Acs.Common.DataType.Binary Payload
//        {
//            get
//            {
//                return (Binary)FieldValues[4];
//            }
//            set
//            {
//                FieldValues[4] = value;
//            }
//        }
//        public short QoS
//        {
//            get
//            {
//                return (short)FieldValues[5];
//            }
//            set
//            {
//                FieldValues[5] = value;
//            }
//        }
//        public bool Retain
//        {
//            get
//            {
//                return (bool)FieldValues[6];
//            }
//            set
//            {
//                FieldValues[6] = value;
//            }
//        }

//        public string MetaDataOmap
//        {
//            get
//            {
//                return (string)FieldValues[7];
//            }
//            set
//            {
//                FieldValues[7] = value;
//            }
//        }
//        public string ClientID
//        {
//            get
//            {
//                return (string)FieldValues[8];
//            }
//            set
//            {
//                FieldValues[8] = value;
//            }
//        }

//        public List<MQTTnet.Packets.MqttUserProperty> UserProperties
//        {
//            get
//            {
//                return (List<MQTTnet.Packets.MqttUserProperty>)FieldValues[9];
//            }
//            set
//            {
//                FieldValues[9] = value;
//            }
//        }
//    }
//}
