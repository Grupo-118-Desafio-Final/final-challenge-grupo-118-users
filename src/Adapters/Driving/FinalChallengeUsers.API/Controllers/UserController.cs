using Application.User;
using Domain.Users.Dto;
using Domain.Users.Ports.In;
using FinalChallengeUsers.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace FinalChallengeUsers.API.Controllers;

/// <summary>
/// Controller responsável por expor endpoints HTTP relacionados a operações de usuário,
/// como autenticação e criação de conta.
/// </summary>
[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserManager _userManager;

    /// <summary>
    /// Inicializa uma nova instância de <see cref="UserController"/>.
    /// </summary>
    /// <param name="userManager">Instância de <see cref="UserManager"/> usada para operações de usuário.</param>
    public UserController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Autentica um usuário usando email e senha e retorna um token de autenticação.
    /// </summary>
    /// <param name="loginParameters">
    /// Parâmetros de login contendo as propriedades <see cref="LoginParameters.Email"/> e <see cref="LoginParameters.Password"/>.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o token de autenticação em caso de sucesso (HTTP 200).
    /// Em caso de falha, o comportamento (códigos de resposta e corpo) segue a implementação de <see cref="UserManager.LoginAsync"/>.
    /// </returns>
    [ProducesResponseType(200)]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginParameters loginParameters)
    {
        var token = await _userManager.LoginAsync(loginParameters.Email, loginParameters.Password);
        return string.IsNullOrEmpty(token) ? Unauthorized() : Ok(token);
    }

    /// <summary>
    /// Cria um novo usuário com os dados fornecidos e retorna o identificador criado.
    /// </summary>
    /// <param name="createUserParameters">
    /// Parâmetros para criação de usuário contendo <see cref="CreateUserParameters.Email"/>, 
    /// <see cref="CreateUserParameters.Password"/>, <see cref="CreateUserParameters.Name"/> e <see cref="CreateUserParameters.LastName"/>.
    /// </param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o identificador do usuário criado (HTTP 200).
    /// Os possíveis códigos de erro e validações dependem da implementação de <see cref="UserManager.CreateUserAsync"/>.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] UserCreateRequestDto userCreateRequestDto)
    {
        var userId = await _userManager.CreateUserAsync(userCreateRequestDto);

        return Ok(userId);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(
        Guid id,
        [FromBody] UserUpdateRequestDto userUpdateRequestDto)
    {
        userUpdateRequestDto.Id = id;
        var userResponseDto = await _userManager.UpdateUserAsync(userUpdateRequestDto);
        return Ok(userResponseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var userResponseDto = await _userManager.DeleteUser(id);

        return Ok(userResponseDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var userResponseDto = await _userManager.GetUserByIdAsync(id);
        return Ok(userResponseDto);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllAsync()
    {
        var userResponseDtoList = await _userManager.GetAllUsersAsync();
        return Ok(userResponseDtoList);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        var userResponseDto = await _userManager.GetUserByEmailAsync(email);
        return Ok(userResponseDto);
    }
}
