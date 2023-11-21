using AutoMapper;
using MongoDB.Driver;
using MyBlogs.Models;
using MyBlogs.DTOModels;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyBlogs.Services;

public class UsersService
{
    private readonly IMongoCollection<Users> _usersCollection;
    private readonly IMapper _mapper;

    public UsersService(IOptions<UsersDatabaseSettings> usersDatabaseSettings, IMapper mapper)
    {
        var mongoClient = new MongoClient(
            usersDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            usersDatabaseSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<Users>(
            usersDatabaseSettings.Value.UsersCollectionName);
        
        _mapper = mapper;
    }

    public async Task<Users> LoginService(UsersLoginDTO loginDto)
    {   
        var users = await _usersCollection.Find(x => x.Username == loginDto.Username).FirstOrDefaultAsync();
        if (users == null)
            throw new Exception("user not found");
        
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: loginDto.Password!,
            salt: users.Salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
           
        if (users.Password != hashed)
        {
            throw new Exception("Password does not match");
        }
        return users;
    }

    public async Task<List<UsersGetDTO>> GetAsync()
    {
        var users = await _usersCollection.Find(_ => true).ToListAsync();
        var res = _mapper.Map<List<UsersGetDTO>>(users);
        return res;
    }

    public async Task<UsersGetDTO?> GetAsync(string id)
    {
        var users = await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        var res = _mapper.Map<UsersGetDTO>(users);   
        return res;
    }
    public async Task<Users> CreateAsync(UsersPostDTO newUserDto)
    {
        var newUser = _mapper.Map<Users>(newUserDto);
    
        await _usersCollection.InsertOneAsync(newUser);
        return newUser;
    }

    public async Task UpdateAsync(Users updateUser)
    {
        await _usersCollection.ReplaceOneAsync(x => x.Id == updateUser.Id, updateUser);
    }

    public async Task RemoveAsync(string id) => await _usersCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<bool> EmailExists(string email)
    {
        var users = await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        return users != null;
    }

    public async Task<bool> UsernameExists(string username)
    {
        var users = await _usersCollection.Find(x => x.Username == username).FirstOrDefaultAsync();
        return users != null;
    }

    public async Task<bool> PhoneExists(string phone)
    {
        var users = await _usersCollection.Find(x => x.Phone == phone).FirstOrDefaultAsync();
        return users != null;
    }
}