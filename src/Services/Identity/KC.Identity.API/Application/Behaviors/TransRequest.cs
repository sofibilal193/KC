
// using Application.Common.Behaviors;
// using Application.Common.Repositories;
// using MediatR;

// namespace Identity.API.Application
// {
//     public abstract class TransRequest<TRequest, TResponse> : ITransRequest<TRequest, TResponse>
//         where TRequest : IRequest<TResponse>
//     {
//         public IDbContext DbContext { get; }

//         public TRequest Request { get; }

//         protected TransRequest(IDbContext context)
//         {
//             DbContext = context;
//         }

//         protected TransRequest(TRequest request, IDbContext context)
//         {
//             Request = request;
//             DbContext = context;
//         }
//     }
// }