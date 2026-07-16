import { ExtractJwt, Strategy } from 'passport-jwt';
import { PassportStrategy } from '@nestjs/passport';
import { Injectable } from '@nestjs/common';
import { ConfigService } from '@nestjs/config';

@Injectable()
export class JwtStrategy extends PassportStrategy(Strategy) {
  constructor(private readonly configService: ConfigService) {
    super({
      jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
      ignoreExpiration: false,
      secretOrKey: configService.get<string>('JWT_SECRET') || 'T8tHezuFsRfZYj5dyhUcMxmvxmldsigmoreeyedpqlakwefzwwd',
    });
  }

  async validate(payload: any) {
    // Map JWT payload properties directly to req.user object
    return {
      userId: payload.userId,
      username: payload.username,
      email: payload.email,
      fullName: payload.fullName,
      role: payload.role,
      iss: payload.iss,
      aud: payload.aud,
      nbf: payload.nbf,
      exp: payload.exp,
      iat: payload.iat,
    };
  }
}
