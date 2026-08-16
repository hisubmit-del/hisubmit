using System;
using System.Linq;
using System.Linq.Expressions;
using HiSubmit.Domain.Contracts;
using System.Collections.Generic;
using System.Reflection;

namespace HiSubmit.Application.Specifications.Base;

public abstract class HeroSpecification<T> : ISpecification<T> where T : class, IEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    public  ISpecification<T> And (ISpecification<T> specify2)
    {
        BinaryExpression d= Expression.AndAlso(this.Criteria.Body, specify2.Criteria.Body);
        this.Criteria = Expression.Lambda<Func<T, bool>>(d, specify2.Criteria.Parameters.Single());
        this.Includes.AddRange(specify2.Includes);
        this.IncludeStrings.AddRange(specify2.IncludeStrings);
        return this;
    }

    public HeroSpecification<T> Or(HeroSpecification<T> specify2)
    {
        BinaryExpression d= Expression.OrElse(this.Criteria, specify2.Criteria);
        this.Criteria = Expression.Lambda<Func<T, bool>>(d, specify2.Criteria.Parameters.Single());
        this.Includes.AddRange(specify2.Includes);
        this.IncludeStrings.AddRange(specify2.IncludeStrings);
        return this;
    }
        
    public HeroSpecification<T> Not()
    {
        UnaryExpression d= Expression.Not(this.Criteria);
        this.Criteria = Expression.Lambda<Func<T, bool>>(d, this.Criteria.Parameters.Single());
        return this;
    }
}

public sealed class AndSpecification<T>:HeroSpecification<T> where T :class,IEntity
{
    public readonly HeroSpecification<T> _leftSpecification;
    public readonly HeroSpecification<T> _rightSpecification;
    public List<HeroSpecification<T>> _Specifications = new();
    public AndSpecification(HeroSpecification<T> leftSpecification, 
        HeroSpecification<T> rightSpecification)
    {
        _leftSpecification = leftSpecification;
        _rightSpecification = rightSpecification;
        //GenerateSpecification();
    }

    public AndSpecification(params HeroSpecification<T>[] sppecifications)
    {
        _Specifications = sppecifications.ToList();
    }

    private void GenerateSpecification()
    {
        BinaryExpression d= Expression.AndAlso(_leftSpecification.Criteria.Body, _rightSpecification.Criteria.Body);
        
        ParameterExpression[] parameters = new ParameterExpression[1] {
        Expression.Parameter(typeof(T), _leftSpecification.Criteria.Parameters.First().Name) };


        Criteria = Expression.Lambda<Func<T, bool>>(d
            , parameters
            //, _rightSpecification.Criteria.Parameters.First()
            );

       var f=Criteria.Compile();

        Includes.AddRange(_leftSpecification.Includes);
        Includes.AddRange(_rightSpecification.Includes);
        IncludeStrings.AddRange(_leftSpecification.IncludeStrings);
        IncludeStrings.AddRange(_rightSpecification.IncludeStrings);
        
    }
}
