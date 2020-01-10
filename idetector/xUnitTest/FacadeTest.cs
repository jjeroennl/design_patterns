using System;
using idetector;
using idetector.Collections;
using idetector.Data;
using idetector.Models;
using idetector.Parser;
using idetector.Patterns.Facade;
using idetector.Patterns.Helper;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace xUnitTest
{
    public class FacadeTest
    {
        private ClassCollection _facade;
        private ClassCollection _notFacade;
        private ClassCollection _reallyNotFacade;

        [Fact]
        void FacadeProject()
        {
            registerFacade();
            Facade f = new Facade(this._facade);
            f.Scan();

            var requirements = new Requirements();
            var calc = new ScoreCalculator(requirements.GetRequirements());
            var scores = calc.GetScore("FACADE", f.GetResults());
            
            Assert.Equal(100, scores["HouseBuilderFacade"]);
        }
        
        [Fact]
        void NotFacade()
        {
            registerFacade();
            Facade f = new Facade(this._notFacade);
            f.Scan();
            var requirements = new Requirements();
            var calc = new ScoreCalculator(requirements.GetRequirements());
            var scores = calc.GetScore("FACADE", f.GetResults());
            
            Assert.Equal(0, scores["HouseBuilderFacade"]);
        }
        
//        [Fact]
//        void ReallyNotFacade()
//        {
//            registerFacade();
//
//            Facade f = new Facade(this._reallyNotFacade);
//            f.Scan();
//            Assert.Equal(0, f.Score("Current"));
//            Assert.Equal(0, f.Score("NoteBs"));
//        }


        void registerFacade()
        {
            var facade = CSharpSyntaxTree.ParseText(@"
using System;

namespace FacadeExample
{
    public class ElectricityCables
    {
        public void ConnectElectricityCables()
        {
            var X = new HelperClass();
            Console.WriteLine(""Electricity cables connected"");
        }
    }
}﻿using System;

namespace FacadeExample
{
    public class GasPipes
    {
        public void ConnectGasPipes()
        {
            Console.WriteLine(""Gas pipes connected"");
        }
    }
}﻿namespace FacadeExample
{
    public class HouseBuilderFacade
    {
        private readonly ElectricityCables _electricityCables;
        private readonly GasPipes _gasPipes;
        private readonly WaterPipes _waterPipes;
        private readonly PoolInstallation _poolInstallation;

        public HouseBuilderFacade()
        {
            _electricityCables = new ElectricityCables();
            _gasPipes = new GasPipes();
            _waterPipes = new WaterPipes();
            _poolInstallation = new PoolInstallation();
        }

        public void BuildHouse()
        {
            _waterPipes.ConnectPipes();
            _gasPipes.ConnectGasPipes();
            _electricityCables.ConnectElectricityCables();
        }

        public void BuildMansion()
        {
            _waterPipes.ConnectPipes();
            _gasPipes.ConnectGasPipes();
            _electricityCables.ConnectElectricityCables();
            _poolInstallation.InstallPool();
        }
    }
}﻿using System;

namespace FacadeExample
{
    public class PoolInstallation
    {
        public void InstallPool()
        {
            Console.WriteLine(""Pool installed"");
        }
    }
}﻿using System;

namespace FacadeExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var facade = new HouseBuilderFacade();

            Console.WriteLine(""Building house"");
            facade.BuildHouse();
            Console.WriteLine(""Building mansion"");
            facade.BuildMansion();

            var X = new HelperClass();

        }
    }
}
﻿using System;


namespace FacadeExample{
    class HelperClass{
        
    }
}

namespace FacadeExample
{
    public class WaterPipes
    {
        public void ConnectPipes()
        {
            Console.WriteLine(""Water pipes connected"");
        }
    }
}

class User: UserParent{
                    private static User me;

                    private User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null){
                        }
                        
                        return this.me;
                    }
                }
            ");

            this._facade = Walker.GenerateModels(facade);

            var notFacade = CSharpSyntaxTree.ParseText(@"
 using System;

namespace FacadeExample
{
    public class ElectricityCables
    {
        public void ConnectElectricityCables()
        {
            var X = new HelperClass();
            Console.WriteLine(""Electricity cables connected"");
        }
    }
}﻿using System;

namespace FacadeExample
{
    public class GasPipes
    {
        public void ConnectGasPipes()
        {
            Console.WriteLine(""Gas pipes connected"");
        }
    }
}﻿namespace FacadeExample
{
    public class HouseBuilderFacade
    {
        private readonly ElectricityCables _electricityCables;
        private readonly GasPipes _gasPipes;
        private readonly WaterPipes _waterPipes;
        private readonly PoolInstallation _poolInstallation;

        public HouseBuilderFacade()
        {
            _electricityCables = new ElectricityCables();
            _gasPipes = new GasPipes();
            _waterPipes = new WaterPipes();
            _poolInstallation = new PoolInstallation();
        }

        public void BuildHouse()
        {
            _waterPipes.ConnectPipes();
            _gasPipes.ConnectGasPipes();
            _electricityCables.ConnectElectricityCables();
        }

        public void BuildMansion()
        {
            _waterPipes.ConnectPipes();
            _gasPipes.ConnectGasPipes();
            _electricityCables.ConnectElectricityCables();
            _poolInstallation.InstallPool();
        }
    }
}﻿using System;

namespace FacadeExample
{
    public class PoolInstallation
    {
        public void InstallPool()
        {
            Console.WriteLine(""Pool installed"");
        }
    }
}﻿using System;

namespace FacadeExample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var facade = new HouseBuilderFacade();

            Console.WriteLine(""Building house"");
            facade.BuildHouse();
            Console.WriteLine(""Building mansion"");
            facade.BuildMansion();
        }
    }
}
﻿using System;


namespace FacadeExample{
    class HelperClass{
        
    }
}

namespace FacadeExample
{
    public class WaterPipes
    {
        public void ConnectPipes()
        {
            Console.WriteLine(""Water pipes connected"");
        }
    }
}

class User: UserParent{
                    private static User me;

                    private User(){

                    }
                    
                    public static User getUser(){
                        if(this.me == null){
                            var X = new HelperClass();
                        }
                        
                        return this.me;
                    }
                }

            ");

            this._notFacade = Walker.GenerateModels(notFacade);
            
                        var reallyNotFacade = CSharpSyntaxTree.ParseText(@"
 namespace idetector.Patterns
{ 
    class User{
            public static User me; 

            private static User(){

            }
                    
            public  User getUser(){
                if(User.me == null){
                    User.me = new User();
                }
                return User.me;
            }
        }

        class Current
        {
            private static Current me; 

            private static Current(){

            }
                    
            public static Current getCurrent(){
                if(Current.me == null){
                    Current.me = new Current();
                }
                return Current.me;
            }
        }

        class Data
        {
            public static Data me; 

            private Data(){

            }
                    
            public static Data getData(){
                if(Data.me == null){
                    Data.me = new Data();
                }
                return Data.me;
            }
        }
}
public abstract class ComponentBase
{
    public abstract void Operation();
}
 
 
public class ConcreteComponent : ComponentBase
{
    public override void Operation()
    {
        Console.WriteLine(""Component Operation"");
    }
}
 
public abstract class DecoratorBase : ComponentBase
{
    private ComponentBase _component;
 
    public DecoratorBase()
    {
        
    }
 
    public override void Operation()
    {
        _component.Operation();
    }
}
 
 
public class ConcreteDecorator : DecoratorBase
{
    public ConcreteDecorator(ComponentBase component) : base(component) { }
 
    public override void Operation()
    {
        base.Operation();
        Console.WriteLine(""(modified)"");
    }
}
namespace PianoTrainer.Controller{
public interface INote {
   INote getNote();
    
   void setNote(INote note):
    }
}

namespace PianoTrainer.Controller {
public class NoteA : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
namespace PianoTrainer.Controller {
public class NoteB : INote{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
public class NoteBs : NoteB{
        INote _note;

        public INote getNote(){
            return note;
        }
        public void SetNote(INote note){
            _note = note;
        }
    }
}
            ");

            this._reallyNotFacade = Walker.GenerateModels(reallyNotFacade);
        }
    }
}