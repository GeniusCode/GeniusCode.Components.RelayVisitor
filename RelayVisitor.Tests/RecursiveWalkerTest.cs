using GeniusCode.Components.RelayVisitor.Tests.Support;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;


namespace GeniusCode.Components.RelayVisitor.Tests
{

    [TestClass]
    public class RecursiveWalkerTest
    {


        [TestMethod]
        public void Should_get_types()
        {
            var typeNames = new List<string>();
            
            var typeWalker = GetRelayVisiterForTypes(typeNames);

            typeWalker.Go(typeof(ObservableCollection<>));

            Assert.AreEqual(11,typeNames.Count);
        }

        private static RelayVisitor<Type> GetRelayVisiterForTypes(List<string> typeNames)
        {
            var typeWalker = new RelayVisitor<Type>(true);
            typeWalker.AddActionForRootType(o => typeNames.Add(o.AssemblyQualifiedName));
            typeWalker.AddScoutForRootType(o => o.BaseType);
            typeWalker.AddScoutForRootType(o => o.GetInterfaces());

            typeWalker.AddActionForRootType(o => Debug.WriteLine(o));

            typeWalker.Log = new DebugWriteLogger();
            return typeWalker;
        }


        [TestMethod()]
        public void Should_get_types_except_for_stop_point()
        {
            var typeNames = new List<string>();
            var visitor = GetRelayVisiterForTypes(typeNames);
            visitor.GoUntil(typeof(ObservableCollection<>),()=> typeNames.Count == 5);
            Assert.AreEqual(5,typeNames.Count());
        }
    }
}
