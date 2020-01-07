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
            singletonreqs.Add(new PatternRequirement("SINGLETON-PRIVATE-CONSTRUCTOR", 1, "Class does not contain a private constructor or the constructor is not private"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-STATIC-SELF", 1, "Class does not have a static field of itself or the field is not static"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-GET-INSTANCE", 1, "Class does not contain a method that returns the instance or returns more then one instance"));
            singletonreqs.Add(new PatternRequirement("SINGLETON-CREATE-SELF", 1, "Class either does not create itself or creates itself multiple times"));
            PatternRequirements.Add("SINGLETON", singletonreqs);


            //DECORATOR
            //ID's:
            //DECORATOR-BASE-HAS-CHILDREN
            //DECORATOR-BASE-CHILDREN-TYPES
            //DECORATOR-HAS-CHILDREN
            //DECORATOR-HAS-BASE-PROPERTY
            //DECORATOR-CONSTRUCTOR-SETS-COMPONENT
            //DECORATOR-CONCRETE-CALLS-BASE
            List<PatternRequirement> decoratorreqs = new List<PatternRequirement>();
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-HAS-CHILDREN", 1, "The base decorator class did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-BASE-CHILDREN-TYPES", 10, "The base decorator did not contain a decorator child, or multiple decorator childs"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-CHILDREN", 1, "The decorator class did not have any children"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-HAS-BASE-PROPERTY", 1, "The decorator class did not contain a field of the parent type"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONSTRUCTOR-SETS-COMPONENT", 1, "The decorator class' constructor does not set component field"));
            decoratorreqs.Add(new PatternRequirement("DECORATOR-CONCRETE-CALLS-BASE", 1, "The concrete decorators call the base decorator's constructor"));
            PatternRequirements.Add("DECORATOR", decoratorreqs);

            /*ID's:
             *FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS
             *FACTORY-CONTAINS-PRODUCT-INTERFACE
             *FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD
             *FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS
             *FACTORY-INHERITING-PRODUCT-INTERFACE
             *FACTORY-RETURNS-PRODUCT         
             *FACTORY-ONE-METHOD
             *FACTORY-ONE-PRODUCT-INTERFACE
             */
            List<PatternRequirement> factoryreqs = new List<PatternRequirement>();
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-FACTORY-CLASS", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-PRODUCT-INTERFACE", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-CONTAINS-ABSTRACT-PRODUCT-INTERFACE-METHOD", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-ABSTRACT-FACTORY-CLASS", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-INHERITING-PRODUCT-INTERFACE", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-RETURNS-PRODUCT", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-ONE-METHOD", 1, ""));
            factoryreqs.Add(new PatternRequirement("FACTORY-ONE-PRODUCT-INTERFACE", 1, ""));
            PatternRequirements.Add("FACTORY", factoryreqs);

            /*ID's:
             * STATE-STRATEGY-CONTEXT-HAS-STRATEGY
             * STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY
             * STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR
             * STATE-STRATEGY-CONTEXT-STRATEGY-SETTER
             * STATE-STRATEGY-CONTEXT-LOGIC
             * STATE-STRATEGY-INTERFACE-ABSTRACT
             * STATE-STRATEGY-CONCRETE-CLASS
             * STATE-CONCRETE-CLASS-RELATIONS
             */
            List<PatternRequirement> strategyreqs = new List<PatternRequirement>();
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-HAS-STRATEGY", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PRIVATE-STRATEGY", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-PUBLIC-CONSTRUCTOR", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-STRATEGY-SETTER", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONTEXT-LOGIC", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-INTERFACE-ABSTRACT", 1, ""));
            strategyreqs.Add(new PatternRequirement("STATE-STRATEGY-CONCRETE-CLASS", 1, ""));
            PatternRequirements.Add("STRATEGY", strategyreqs);

            /*ID's:
             * OBSERVER-HAS-INTERFACE-WITH-NAMED-UPDATE-FUNCTION
             * OBSERVER-HAS-INTERFACE-WITH-UPDATE-FUNCTION
             * OBSERVER-HAS-SUBJECT-FUNCTIONS
             * OBSERVER-HAS-SUBJECT-WITH-OBSERVER-LIST
             * OBSERVER-CONCRETE-OBSERVER-EXTENDS-IOBSERVERS
             */
            List<PatternRequirement> observerreqs = new List<PatternRequirement>();
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-INTERFACE-WITH-NAMED-UPDATE-FUNCTION", 1, "No interface with a void function named Update exists."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-INTERFACE-WITH-UPDATE-FUNCTION", 1, "No interface with a void function exists."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-SUBJECT-FUNCTIONS", 1, "No interface with correlating subject functions exists."));
            observerreqs.Add(new PatternRequirement("OBSERVER-HAS-SUBJECT-WITH-OBSERVER-LIST", 1, "No interface with a list of interfaces exists."));
            observerreqs.Add(new PatternRequirement("OBSERVER-CONCRETE-OBSERVER-EXTENDS-IOBSERVERS", 1, "There arent any classes that extend IObserver."));
            PatternRequirements.Add("OBSERVER", observerreqs);


            var statereqs = strategyreqs;
            statereqs.Add(new PatternRequirement("STATE-CONCRETE-CLASS-RELATIONS", 10, ""));
            PatternRequirements.Add("STATE", statereqs);



        }
        public Dictionary<string, List<PatternRequirement>> GetRequirements()
        {
            Console.WriteLine("hoi");
            return PatternRequirements;
        }

    }
}
