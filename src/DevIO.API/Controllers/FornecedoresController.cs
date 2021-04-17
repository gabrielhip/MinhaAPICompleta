using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.API.ViewModels;
using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.API.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        public FornecedoresController(IFornecedorRepository fornecedorRepository, 
                                      IMapper mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }

        //quando o método é async ele tem que retornar uma Task<T>
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedor = await _fornecedorRepository.ObterTodos(); //await espera até ter o resultado do método. Se não tivesse await, retornaria uma Task<>.
            var fornecedorVM = _mapper.Map<IEnumerable<FornecedorViewModel>>(fornecedor); //mapeando objeto Fornecedor para FornecedorViewModel
            return Ok(fornecedorVM);
        }  
    }
}