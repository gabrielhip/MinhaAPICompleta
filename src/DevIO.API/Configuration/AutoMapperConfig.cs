using AutoMapper;
using DevIO.API.ViewModels;
using DevIO.Business.Models;

namespace DevIO.API.Configuration
{
    public class AutoMapperConfig : Profile //herdando de uma classe de Perfil (do próprio automapper)
    {
        public AutoMapperConfig()
        {
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap(); //se o mapeamento for igual nos dois sentidos usar .ReverseMap()
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
            
            CreateMap<ProdutoViewModel, Produto>();

            CreateMap<ProdutoImagemViewModel, Produto>().ReverseMap();

            //atribui o nome do fornecedor da classe modal para a viewmodel
            CreateMap<Produto, ProdutoViewModel>()
                .ForMember(dest => dest.NomeFornecedor,
                    opt => opt.MapFrom(src => src.Fornecedor.Nome));
        }
    }
}