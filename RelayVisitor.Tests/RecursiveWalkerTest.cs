
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using GeniusCode.Framework.Support.Objects;
namespace GcCore.Tests
{

    [TestClass()]
    public class RecursiveWalkerTest
    {


        [TestMethod()]
        public void Simple_Go_Test()
        {
            List<string> TypeNames = new List<string>();
            
            RelayVisitor<Type> typeWalker = new RelayVisitor<Type>(true);
            typeWalker.AddActionForRootType(o => TypeNames.Add(o.AssemblyQualifiedName));
            typeWalker.AddScoutForRootType(o => o.BaseType);
            typeWalker.AddScoutForRootType(o => o.GetInterfaces());

            typeWalker.AddActionForRootType(o=> Debug.WriteLine(o));

            typeWalker.Log = new DebugWriteLogger();

            typeWalker.Go(typeof(ObservableCollection<>));

            Assert.AreEqual(11,TypeNames.Count);
        }


        [TestMethod()]
        public void GoTest_WithStopPoint()
        {
            List<string> TypeNames = new List<string>();

            RelayVisitor<Type> visitor = new RelayVisitor<Type>(true);
            
            visitor.AddScoutForRootType(o => o.BaseType, null);
            visitor.AddScoutForRootType(o => o.GetInterfaces().ToList(), null);


            visitor.AddActionForRootType(o => TypeNames.Add(o.AssemblyQualifiedName));
            visitor.AddActionForRootType(o => Debug.WriteLine(o));

            visitor.GoUntil(typeof(ObservableCollection<>),()=> TypeNames.Count == 1);

            Assert.AreEqual(1,TypeNames.Count());
        }
    }
}
