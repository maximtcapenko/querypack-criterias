﻿namespace QueryPack.Criterias.Impl
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class GenericCriteriaBuilder<TEntity, TModel> : ICriteriaBuilder<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        private readonly Dictionary<Operator, List<Criteria>> _criterias = new Dictionary<Operator, List<Criteria>>();

        internal enum Operator
        {
            And,
            Or
        }

        class GroupCriteriaConfigurator : IGroupCriteriaConfigurator<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias = new Dictionary<Operator, List<Criteria>>();

            public IScopeCriteriaConfigurator<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestriction<TModel>> restriction = null)
            {
                var criteriaBuilder = new ScopeCriteriaConfigurator(_criterias);
                criteriaBuilder.And(predicateFactory, restriction);
                return criteriaBuilder;
            }

            public Expression<Func<TEntity, bool>> Build(TModel model)
                => GenericCriteriaBuilder<TEntity, TModel>.Build(_criterias, model);
        }

        class ScopeCriteriaConfigurator : IScopeCriteriaConfigurator<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias;

            public ScopeCriteriaConfigurator(Dictionary<Operator, List<Criteria>> criterias)
            {
                _criterias = criterias;
            }

            public IScopeCriteriaConfigurator<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestriction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public IScopeCriteriaConfigurator<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
                Action<IRestriction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }
        }

        class CriteriaConfigurator : ICriteriaConfigurator<TEntity, TModel>
        {
            private readonly Dictionary<Operator, List<Criteria>> _criterias;

            public CriteriaConfigurator(Dictionary<Operator, List<Criteria>> criterias)
            {
                _criterias = criterias;
            }

            public ICriteriaConfigurator<TEntity, TModel> And(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestriction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.And, out var predicates))
                    predicates.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public ICriteriaConfigurator<TEntity, TModel> And(Action<IGroupCriteriaConfigurator<TEntity, TModel>> block)
            {
                var builder = new GroupCriteriaConfigurator();
                block(builder);
                var criteria = new Criteria(m => builder.Build(m));
                if (_criterias.TryGetValue(Operator.And, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.And, new List<Criteria> { criteria });

                return this;
            }

            public ICriteriaConfigurator<TEntity, TModel> Or(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory, Action<IRestriction<TModel>> restriction = null)
            {
                var criteria = new Criteria(predicateFactory);
                if (_criterias.TryGetValue(Operator.Or, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                restriction?.Invoke(criteria);

                return this;
            }

            public ICriteriaConfigurator<TEntity, TModel> Or(Action<IGroupCriteriaConfigurator<TEntity, TModel>> block)
            {
                var builder = new GroupCriteriaConfigurator();
                block(builder);
                var criteria = new Criteria(m => builder.Build(m));
                if (_criterias.TryGetValue(Operator.Or, out var criterias))
                    criterias.Add(criteria);
                else
                    _criterias.Add(Operator.Or, new List<Criteria> { criteria });

                return this;
            }
        }

        class Criteria : IRestriction<TModel>
        {
            private readonly List<Expression<Func<TModel, bool>>> _restrictions = new List<Expression<Func<TModel, bool>>>();
            private readonly Func<TModel, Expression<Func<TEntity, bool>>> _predicateFactory;
            private Func<TModel, bool> _compiledRestriction;

            public Criteria(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory)
            {
                _predicateFactory = predicateFactory;
            }

            public void When(Expression<Func<TModel, bool>> restriction)
            {
                _restrictions.Add(restriction);
            }

            public Expression<Func<TEntity, bool>> GetWhenValid(TModel model)
            {
                if (_restrictions.Any())
                {
                    if (_compiledRestriction == null)
                    {
                        var expression = _restrictions[0];
                        for (int i = 1; i < _restrictions.Count; i++)
                        {
                            expression = expression.And(_restrictions[i]);
                        }

                        _compiledRestriction = expression.Compile();
                    }

                    if (_compiledRestriction(model))
                        return _predicateFactory(model);
                    else
                        return null;
                }

                return _predicateFactory(model);
            }
        }

        public ICriteriaConfigurator<TEntity, TModel> With(Func<TModel, Expression<Func<TEntity, bool>>> predicateFactory,
            Action<IRestriction<TModel>> restriction = default)
        {
            var criteriaBuilder = new CriteriaConfigurator(_criterias);
            criteriaBuilder.And(predicateFactory, restriction);

            return criteriaBuilder;
        }

        static Expression<Func<TEntity, bool>> Build(IDictionary<Operator, List<Criteria>> criterias, TModel model)
        {
            if (!criterias.Any())
                return e => true;

            var validCriterias = new Dictionary<Operator, List<Expression<Func<TEntity, bool>>>>();

            foreach (var key in criterias.Keys)
            {
                var validPredicates = new List<Expression<Func<TEntity, bool>>>();

                foreach (var criteria in criterias[key])
                {
                    var validPredicate = criteria.GetWhenValid(model);
                    if (validPredicate != null)
                        validPredicates.Add(validPredicate);
                }

                validCriterias.Add(key, validPredicates);
            }

            if (!validCriterias.Any())
                return e => true;

            Expression<Func<TEntity, bool>> first = null;

            foreach (var key in validCriterias.Keys)
                foreach (var predicate in validCriterias[key])
                {
                    first = key == Operator.And
                        ? first.And(predicate) : first.Or(predicate);
                }

            if (first == null)
                return e => true;

            return first;
        }

        public Expression<Func<TEntity, bool>> Build(TModel model) => Build(_criterias, model);
    }
}
