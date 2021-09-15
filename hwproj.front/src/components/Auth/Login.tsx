import React, {FC, FormEvent} from "react";
import Avatar from '@material-ui/core/Avatar';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined'
import {Redirect, RouteComponentProps} from "react-router-dom";
import { TextField, Button, Typography, CssBaseline } from "@material-ui/core";
import Grid from '@material-ui/core/Grid';
import ApiSingleton from "../../api/ApiSingleton";
import "./Styles/Login.css";
import { useState } from "react";
import { red } from '@material-ui/core/colors';
import { LoginViewModel } from "../../api/"
import makeStyles from "@material-ui/styles/makeStyles";
import Container from '@material-ui/core/Container';
import GoogleLogin from "react-google-login";

interface LoginProps extends Partial<RouteComponentProps> {
  onLogin: () => void;
}

interface ILoginState {
  email: string;
  password: string;
  error: string[] | null;
  isLogin: boolean;
}

const useStyles = makeStyles((theme) => ({
  paper: {
    marginTop: theme.spacing(8),
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
  },
  avatar: {
    margin: theme.spacing(1),
  },
  form: {
    marginTop: theme.spacing(3),
    width: '100%'
  },
  button: {
    marginTop: theme.spacing(1)
  },
}))

const Login: FC<LoginProps> = (props) => {
  const classes = useStyles()
  const [loginState, setLoginState] = useState<ILoginState>({
    email: '',
    password: '',
    error: [],
    isLogin: ApiSingleton.authService.loggedIn(),
  })

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const userData : LoginViewModel = {
      email: loginState.email,
      password: loginState.password,
      rememberMe: false
    }
    try{
      const result = await ApiSingleton.authService.login(userData)
      setLoginState(prevState => ({
        ...prevState,
        error: result.error,
        isLogin: result.isLogin,
      }))
      props.onLogin?.()
    }
    catch (e){
      setLoginState(prevState => ({
        ...prevState,
        error: ['Сервис недоступен'],
        isLogin: false
      }))
    }
  }

  const handleChangeEmail = (e: React.ChangeEvent<HTMLInputElement>) => {
    e.persist()
    setLoginState((prevState) => ({
      ...prevState,
      email: e.target.value
    }))
  }

  const handleChangePassword = (e: React.ChangeEvent<HTMLInputElement>) => {
    e.persist()
    setLoginState((prevState) => ({
      ...prevState,
      password: e.target.value
    }))
  }

  const googleResponse = async (response: any) => {
    const result = await ApiSingleton.authService.loginByGoogle(response.tokenId)
    setLoginState(prevState => ({
      ...prevState,
      error: result!.error,
      isLogin: result.isLogin
    }))
    props.onLogin?.()
  }

  const headerStyles: React.CSSProperties = { marginRight: "9.5rem" };
  const clientId = process.env.REACT_APP_GOOGLE_CLIENT_ID!

  if (loginState.isLogin){
    return <Redirect to={"/"} />;
  }

  if (loginState.error) {
    headerStyles.marginBottom = "-1.5rem";
  }

  return (
      <Container component="main" maxWidth="xs">
        <Grid container className={classes.paper}>
          <Avatar className={classes.avatar} style={{ color: 'white', backgroundColor: '#ba2e2e' }}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5">
            Войти
          </Typography>
          {loginState.error && (
              <p style={{ color: "red", marginBottom: "0" }}>
                {loginState.error}
              </p>
          )}
        </Grid>
        <form onSubmit={(e) => handleSubmit(e)} className={classes.form}>
          <Grid container direction="column" justifyContent="center">
            <Grid>
              <TextField
                  required
                  type="email"
                  fullWidth
                  label="Email Address"
                  variant="outlined"
                  margin="normal"
                  name={loginState.email}
                  onChange={handleChangeEmail}
              />
            </Grid>
            <Grid>
              <TextField
                  required
                  type="password"
                  fullWidth
                  label="Password"
                  variant="outlined"
                  margin="normal"
                  value={loginState.password}
                  onChange={handleChangePassword}
              />
            </Grid>
            <Grid className={classes.button}>
              <Button
                  fullWidth
                  variant="contained"
                  color="primary"
                  type="submit"
              >
                Войти
              </Button>
            </Grid>
          </Grid>
        </form>
        <Typography>
          <hr/>Или войдите с помощью других сервисов<hr/>
        </Typography>
          <GoogleLogin
              clientId={clientId}
              buttonText=''
              onSuccess={googleResponse}
          />
      </Container>
  );
};

export default Login
