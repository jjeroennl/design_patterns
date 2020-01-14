using idetector.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace idetector.Data
{
    public class Requirements
    {
        Dictionary<string, List<PatternRequirement>> PatternRequirements = new Dictionary<string, List<PatternRequirement>>();

        public Requirements()
        {

            //SINGLETON
            /*ID's:
             * SINGLETON-PRIVATE-CONSTRUCTOR
             * SINGLETON-STATIC-SELF
             * SINGLETON-GET-INSTANCE
             * SINGLETON-CREATE-SELF
             */
            List<PatternRequirement> singletonreqs = new List<PatternRequirement>();
            singletonreqs.Add(new PatternRequirement("SINGLETON-PRIVATE-CONSTRUCTOR", "Private constructor", "Because Singletons should not be created from outside the singleton the constructor should not be accessible.", "Class does not contain a private constructor, please create a private constructor"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-STATIC-SELF", "Static field", "To save the instance of the object, a Singleton should contain a private property to store the instance.", "Class does not have a static field of itself or the field is not static"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-GET-INSTANCE", "GetInstance Method", "In order to return the instance, a GetInstance method is needed", "Class does not contain a method that returns the instance or returns more then one instance"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-CREATE-SELF", "Object creation", "A instance should be created inside the Singleton", "Class either does not create itself or creates itself multiple times"));
            PatternRequirements.Add("SINGLETON", singletonreqs);


            //DECORATOR
            //ID's:
            //DECORATOR-BASE-HAS-CHILDREN
            //DECORATOR-HAS-CHILDREN
            //DECORATOR-HAS-BASE-PROPERTY
            //DECORATOR-CONSTRUCTOR-SETS-COMPONENT
            //DECORATOR-CONCRETE-CALLS-BASE
            List<PatternRequirement> decoratorreqs = new List<PatternRequirement>();
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-HAS-CHILDREN", "Base classes", "A decorator pattern should contain an interface or abstract class with atleast 2 children: 1 base class and an abstract decorato.r", "The base interface/abstract did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-CHILDREN", "Abstract decorator implemented", "The abstract decorator class should have atleast 1 child which should be the concrete decorator.", "The abstract decorator did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-BASE-PROPERTY", "Abstract decorator contains parent property", "The abstract decorator class should contain a property of it's parent type to use as a base item.", "The abstract decorator did not contain a field of the parent type"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", "Abstract decorator sets parent property", "The abstract decorator's constructor should set the parent property. This is to pass the base product to the decorator so that the concrete decorators can add to it.", "The abstract decorator' constructor does not set component field"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONCRETE-CALLS-BASE", "Concrete decorator calls base", "The concrete decorator should always call the base constructor so that the base product is set.", "The concrete decorators do not call the abstract decorator's constructor"));
            PatternRequirements.Add("DECORATOR", decoratorreqs);

            //ABSTRACT FACTORY METHOD
            /*ID's:
             *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
             *FACTORY-CONTAINS-PRODUCT-INTERFACE
             *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
             *FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS
             *FACTORY-INHERITING-PRODUCT-INTERFACE
             *FACTORY-RETURNS-PRODUCT         
             *FACTORY-MULTIPLE-METHODS
             */
            List<PatternRequirement> factoryreqs = new List<PatternRequirement>();
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-PRODUCT-INTERFACE", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-PRODUCT-INTERFACE", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS", "", "", ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", "", "", ""));
            PatternRequirements.Add("FACTORY", factoryreqs);

            //ABSTRACT-FACTORY
            /*ID's:
             *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
             *FACTORY-CONTAINS-PRODUCT-INTERFACE
             *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
             *FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS
             *FACTORY-INHERITING-PRODUCT-INTERFACE
             *FACTORY-RETURNS-PRODUCT         
             *FACTORY-MULTIPLE-METHODS
             */
            List<PatternRequirement> absfactoryreqs = new List<PatternRequirement>();
            absfactoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-PRODUCT-INTERFACE", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-PRODUCT-INTERFACE", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS", "", "", ""));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", "", "", ""));
            PatternRequirements.Add("ABSTRACT-FACTORY", absfactoryreqs);

            //STATESTRATEGY
            /*ID's:
             * STATE-STRATEGY-HAS-CONTEXT
             * STATE-STRATEGY-CONTEXT-HAS-STRATEGY
             * STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY
             * STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR
             * STATE-STRATEGY-CONTEXT-STRATEGY-SETTER
             * STATE-STRATEGY-CONTEXT-LOGIC
             * STATE-STRATEGY-INTERFACE-ABSTRACT
             * STATE-STRATEGY-CONCRETE-CLASS
             * STATE-CONCRETE-CLASS-RELATIONS
             */
            List<PatternRequirement> statereqs = new List<PatternRequirement>();
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-INTERFACE-ABSTRACT", "Parent classes", "There should be an parent class", "There are no interfaces or abstract classes found"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-HAS-CONTEXT", "Context class", "There should be an context class, to call all the States/Strategies", "There is no class which suffices to be an Context class"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", "State/Strategy implementation", "In the Context class, there should be an implementation of the State/Strategy", "There is either no class which suffices to be an Context class, or in the Context class there is no implementation of the State/Strategy"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", "Private State/Strategy implementation", "In the Context class, there should be an private implementation of the State/Strategy", "There is either no class which suffices to be an Context class, or no implementation of the State/Strategy, or that implementation is not private"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", "Context public constructor", "The Context class should have an public constructor", "There is either no class which suffices to be an Context class, or the context class has no public constructor"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", "State/Strategy setter", "In the Context class it should contain an setter for the State/Strategy", "There is either no class which suffices to be an Context class, or there is no setter for the State/Strategy in the Context class"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-LOGIC", "Context logic", "The Context class should contain any logic for the State/Strategy to utilize", "There is either no class which suffices to be an Context class, or the Context class contains no logic that call the States/Strategies"));
            statereqs.Add(new PatternRequirement("STATE-STRATEGY-CONCRETE-CLASS", "Concrete classes", "There should be concrete classes which implement the interface or abstract class: State/Strategy", "There is either no class which suffices to be an Context class, or"));
            PatternRequirements.Add("STATE", statereqs);

            var strategyreqs = new List<PatternRequirement>(statereqs);
            strategyreqs.Add(new PatternRequirement("STRATEGY-CONCRETE-CLASS-RELATIONS", "Concrete classes relations", "The concrete classes should have no relation with each other", "The concrete classes, which implement the Strategy, should not call each other in the code"));
            PatternRequirements.Add("STRATEGY", strategyreqs);

            //FACADE
            /*ID's:
             * FACADE-IS-FACADE
             */
            List<PatternRequirement> facadereqs = new List<PatternRequirement>();
            facadereqs.Add(new PatternRequirement("FACADE-IS-FACADE", "Facade detected", "This class has been detected as a Facade, this means it is part of a group of classes that only interact with it or each other.", ""));
            PatternRequirements.Add("FACADE", facadereqs);

            //Command
            /*ID's:
             * COMMAND-HAS-INTERFACE
             * COMMAND-HAS-COMMAND-CLASS
             * COMMAND-HAS-PUBLIC-CONSTRUCTOR
             * COMMAND-HAS-RECEIVER-CLASS
             * COMMAND-USES-RECEIVER
             * COMMAND-HAS-INVOKER-CLASS
             */
            List<PatternRequirement> commandreqs = new List<PatternRequirement>();
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INTERFACE", "Interface or Abstract", "The pattern should contain an interface for all the commands.", "The command pattern does not contain either an interface or an abstract class."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-COMMAND-CLASS", "Command classes", "The pattern should contain command classes in order to execute functions through them.", "There are no commands that implement the abstract class or interface."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-PUBLIC-CONSTRUCTOR", "Command public constructor", "The pattern should contain a public constructor so this can be called through the interface.", "One or all of the commands do not have a public constructor."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-RECEIVER-CLASS", "Receiver classes", "The pattern should contain one or more receiver classes to perform commands on.", "There are no receiver classes implemented"));
            commandreqs.Add(new PatternRequirement("COMMAND-USES-RECEIVER", "Command implements Receiver", "At least one command pattern should implement the receiver in order to perform functions on the designated receiver.", "None of the commands are controlled by a receiver"));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INVOKER-CLASS", "Invoker class", "The pattern should contain an invoker class to perform the commands.", "The code does not contain an Invoker class"));
            PatternRequirements.Add("COMMAND", commandreqs);
        }
        public Dictionary<string, List<PatternRequirement>> GetRequirements()
        {
            return PatternRequirements;
        }

    }
}
