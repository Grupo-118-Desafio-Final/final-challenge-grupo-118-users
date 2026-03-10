using Domain.Users.Dto;
using Domain.Users.Ports.In;
using FinalChallengeUsers.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace FinalChallengeUsers.API.Controllers;

/// <summary>
/// Controller responsável por expor endpoints HTTP relacionados ao gerenciamento de usuários,
/// como autenticação, criação, atualização e exclusão de conta.
/// </summary>
[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserManager _userManager;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="UserController"/>.
    /// </summary>
    /// <param name="userManager">Instância de <see cref="IUserManager"/> usada para operações de usuário.</param>
    public UserController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Autentica um usuário usando e-mail e senha e retorna um token JWT.
    /// </summary>
    /// <param name="loginParameters">Parâmetros de login contendo e-mail e senha.</param>
    /// <returns>
    /// HTTP 200 com o token JWT em caso de sucesso, ou HTTP 401 em caso de credenciais inválidas.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginParameters loginParameters)
    {
        var token = await _userManager.LoginAsync(loginParameters.Email, loginParameters.Password);

        // JWTs são sempre compostos por três segmentos separados por ponto
        return token.StartsWith("ey") ? Ok(token) : Unauthorized(new { message = token });
    }

    /// <summary>
    /// Cria um novo usuário com os dados fornecidos.
    /// </summary>
    /// <param name="userCreateRequestDto">Dados do usuário a ser criado.</param>
    /// <returns>HTTP 201 com os dados do usuário criado.</returns>
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create([FromBody] UserCreateRequestDto userCreateRequestDto)
    {
        var user = await _userManager.CreateUserAsync(userCreateRequestDto);

        return Created(string.Empty, user);
    }

    /// <summary>
    /// Atualiza os dados de um usuário existente.
    /// </summary>
    /// <param name="id">Identificador do usuário a ser atualizado.</param>
    /// <param name="userUpdateRequestDto">Novos dados do usuário.</param>
    /// <returns>
    /// HTTP 200 com os dados atualizados, ou HTTP 404 caso o usuário não seja encontrado.
    /// </returns>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UserUpdateRequestDto userUpdateRequestDto)
    {
        userUpdateRequestDto.Id = id;
        var result = await _userManager.UpdateUserAsync(userUpdateRequestDto);

        if (result.Error)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result);
    }

    /// <summary>
    /// Remove um usuário pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do usuário a ser removido.</param>
    /// <returns>
    /// HTTP 200 com os dados do usuário removido, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _userManager.DeleteUser(id);

        if (result.Error)
            return NotFound(new { message = result.ErrorMessage });

        return Ok(result);
    }

    /// <summary>
    /// Retorna um usuário pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador do usuário.</param>
    /// <returns>
    /// HTTP 200 com os dados do usuário, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _userManager.GetUserByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Retorna todos os usuários cadastrados.
    /// </summary>
    /// <returns>HTTP 200 com a lista de usuários.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _userManager.GetAllUsersAsync();

        return Ok(result);
    }

    /// <summary>
    /// Retorna um usuário pelo seu e-mail.
    /// </summary>
    /// <param name="email">E-mail do usuário.</param>
    /// <returns>
    /// HTTP 200 com os dados do usuário, ou HTTP 404 caso não seja encontrado.
    /// </returns>
    [HttpGet("email/{email}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        var result = await _userManager.GetUserByEmailAsync(email);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
