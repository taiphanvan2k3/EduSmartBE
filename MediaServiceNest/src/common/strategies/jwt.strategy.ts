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
      secretOrKey:
        configService.get<string>('JWT_SECRET') ||
        'T8tHezuFsRfZYj5dyhUcMxmvxmldsigmoreeyedpqlakwefzwwd',
    });
  }

  validate(payload: Record<string, any>) {
    // Map JWT payload properties directly to req.user object
    return {
      userId: payload.userId as number,
      username: payload.username as string,
      email: payload.email as string,
      fullName: payload.fullName as string,
      role: payload.role as string,
      iss: payload.iss as string,
      aud: payload.aud as string,
      nbf: payload.nbf as number,
      exp: payload.exp as number,
      iat: payload.iat as number,
    };
  }
}
