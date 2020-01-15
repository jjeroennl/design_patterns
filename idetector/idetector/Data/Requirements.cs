﻿using idetector.Models;
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
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-HAS-CHILDREN", 
                "Interface is implemented",
                "A decorator pattern should contain an interface with atleast 2 children: 1 base class and an abstract decorator", 
                "A decorator pattern should contain an interface with atleast 2 children: 1 base class and an abstract decorator, please make sure both the base class and the abstract decorator implement the interface"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-CHILDREN", 
                "Abstract decorator implemented", 
                "An abstract decorator should be implemented by one or more concrete decorator(s) so that data or logic can be added to the base item by casting the base item to the concrete decorator type",
                "Please make sure the abstract decorator is implemented by atleast one or more concrete decorators. A class cannot be cast to the abstract decorator type."));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-BASE-PROPERTY", 
                "Abstract decorator contains parent property", 
                "The abstract decorator class should contain a property of it's parent type to use as a base item.", 
                "The abstract decorator did not contain a field of the parent type"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-METHOD-SETS-COMPONENT", 
                "Abstract decorator sets parent property", 
                "The abstract decorator's constructor should set the parent property. This is to pass the base product to the decorator so that the concrete decorators can add to it.", 
                "The abstract decorator' constructor does not set component field"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONCRETE-METHOD-SETS-PROPERTY", 
                "Concrete decorator calls base", 
                "The concrete decorator has a method that sets the class' property of the interface type. This is in order to add data or logic to the saved base object in the concrete decorator", 
                "The concrete decorator should have a method that saves a base object in order for it to add data or logic to it"));
            PatternRequirements.Add("DECORATOR", decoratorreqs);

            //FACTORY METHOD
            /*ID's:
            FACTORY-CONCRETE-FACTORY
            FACTORY-CONCRETE-PRODUCTS
            FACTORY-RETURNS-PRODUCT
            FACTORY-ONE-PRODUCT-INTERFACE
            FACTORY-MULTIPLE-METHODS
             */
            List<PatternRequirement> factoryreqs = new List<PatternRequirement>();
            factoryreqs.Add(new PatternRequirement("FACTORY-CONCRETE-FACTORY",
                "Factory Classes",
                "A factory method should have an interface or abstract class for concrete factories to implement. " +
                "This way the interface/abstract can be used as a guideline for creating a concrete product through different factories",
                "The factory pattern did not contain any concrete factory classes that implemented the factory interface or abstract class"));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONCRETE-PRODUCTS",
                "Concrete Products",
                "A factory method should have concrete products that implement a certain product interface." +
                " The product interface is needed for the factory classes to be able to return a generic product",
                "The factory method did not have a product interface, " +
                "or the product interface was not implemented into a concrete product class"));
            factoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT",
                "Creates Products",
                "A concrete factory should return a concrete product. " +
                "The purpose of a factory method is to create multiple types of one product by using different concrete factories that return different concrete products",
                "Not all concrete factories return a concrete product, or there are no concrete factories/products. " +
                "The purpose of a factory method is to create multiple types of one product by using different concrete factories that return different concrete products"));
            factoryreqs.Add(new PatternRequirement("FACTORY-ONE-PRODUCT-INTERFACE",
                "Singular Product",
                "A factory method can only have a singular product. While it can have multiple types of this product, it cannot have multiple products." +
                " If multiple products are necessary, consider using an abstract factory",
                "Having multiple products in a factory is not allowed. A factory can have multiple types of a product, but not multiple products. Consider using " +
                "abstract factory if multiple products are required"));
            factoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS",
                "One Method",
                "A concrete factory is only allowed to have one method that returns a concrete product." +
                "Concrete factories are as the name implies concrete, and should always return only one concrete product. " +
                "If a factory is required to return more then one concrete product, consider using abstract factory",
                "One or more concrete factories return no or more then one concrete product. If " +
                "a factory is required to return more then one concrete product, consider using abstract factory"));
            PatternRequirements.Add("FACTORY", factoryreqs);

            //ABSTRACT-FACTORY
            /*ID's:
            FACTORY-CONCRETE-FACTORY
            FACTORY-CONCRETE-PRODUCTS
            FACTORY-RETURNS-PRODUCT
            FACTORY-ONE-PRODUCT-INTERFACE
            FACTORY-MULTIPLE-METHODS
             */
            List<PatternRequirement> absfactoryreqs = new List<PatternRequirement>();

            absfactoryreqs.Add(new PatternRequirement("FACTORY-CONCRETE-FACTORY",
                "Factory Classes",
                "An abstract factory should have an interface or abstract class for concrete factories to implement. " +
                "This way the interface/abstract can be used as a guideline for creating a concrete product through different factories",
                "The abstract factory did not contain any concrete factory classes that implemented the factory interface or abstract class"));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-CONCRETE-PRODUCTS",
                "Concrete Products",
                "An abstract factory should have concrete products that implement a certain product interface." +
                " The product interface is needed for the factory classes to be able to return a generic product",
                "The abstract factory did not have a product interface, " +
                "or the product interface was not implemented into a concrete product class"));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT",
                "Creates Products",
                "A concrete factory should return a concrete product. " +
                "The purpose of an abstract factory method is to create multiple types of certain products by using different concrete factories that return different concrete products",
                "Not all concrete factories return a concrete product, or there are no concrete factories/products. " +
                "The purpose of a factory method is to create multiple types of one product by using different concrete factories that return different concrete products"));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-ONE-PRODUCT-INTERFACE",
                "Singular Product",
                "An abstract factory can implement multiple products in the same factory as opposed to factory that can only implement one" +
                " If factories should only return one type of product, consider using regular factory",
                "One or more concrete factories returned one or less products. If only one product is required, consider using a regular factory method"));
            absfactoryreqs.Add(new PatternRequirement("FACTORY-MULTIPLE-METHODS",
                "One Method",
                "A concrete factory is only allowed to have one method that returns a concrete product." +
                "Concrete factories are as the name implies concrete, and should always return only one concrete product. " +
                "If a factory is required to return more then one concrete product, consider using abstract factory",
                "One or more concrete factories return no or more then one concrete product. If " +
                "a factory is required to return more then one concrete product, consider using abstract factory"));
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
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INTERFACE", "Interface or Abstract","The pattern should contain an interface for all the commands.", "The command pattern does not contain either an interface or an abstract class."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-COMMAND-CLASS", "Command classes","The pattern should contain command classes in order to execute functions through them.", "There are no commands that implement the abstract class or interface."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-PUBLIC-CONSTRUCTOR", "Command public constructor","The pattern should contain a public constructor so this can be called through the interface.", "One or all of the commands do not have a public constructor."));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-RECEIVER-CLASS","Receiver classes" ,"The pattern should contain one or more receiver classes to perform commands on.","There are no receiver classes implemented"));
            commandreqs.Add(new PatternRequirement("COMMAND-USES-RECEIVER", "Command implements Receiver", "At least one command pattern should implement the receiver in order to perform functions on the designated receiver.", "None of the commands are controlled by a receiver"));
            commandreqs.Add(new PatternRequirement("COMMAND-HAS-INVOKER-CLASS", "Invoker class","The pattern should contain an invoker class to perform the commands.", "The code does not contain an Invoker class"));
            PatternRequirements.Add("COMMAND", commandreqs);

            //Observer
            /*ID's:
             * OBSERVER-HAS-OBSERVER-INTERFACE
             * OBSERVER-HAS-SUBJECT-INTERFACE
             * OBSERVER-HAS-OBSERVER-RELATIONS
             * OBSERVER-HAS-OBSERVER-AND-SUBJECTS
             */
            List<PatternRequirement> observerreqs = new List<PatternRequirement>();
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-OBSERVER-INTERFACE", "Observer interface", "The pattern should contain an observer interface.", "The command pattern does not contain an observer interface."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-SUBJECT-INTERFACE", "Subject interface", "The pattern should contain a subject interface.", "The pattern should contain a subject interface."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-OBSERVERS-AND-SUBJECTS", "Concrete observer(s) and subject(s)", "The pattern should contain at least one subject and at least one observer.", "There are no concrete observers and subjects."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-OBSERVER-RELATIONS", "Observer relations", "The pattern should have a one-to-many relationship between the subject and its observer(s).", "There is no relation between the potential subject and its observer(s)."));


            PatternRequirements.Add("OBSERVER", observerreqs);
        }
        public Dictionary<string, List<PatternRequirement>> GetRequirements()
        {
            return PatternRequirements;
        }
    }
}
