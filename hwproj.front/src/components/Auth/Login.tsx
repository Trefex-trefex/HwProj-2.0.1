import React, { FormEvent } from "react";
import { RouteComponentProps } from "react-router-dom";
import { TextField, Button, Typography } from "@material-ui/core";
import ApiSingleton from "../../api/ApiSingleton";
import "./Styles/Login.css";

interface LoginProps extends Partial<RouteComponentProps> {
  onLogin: () => void;
}

interface ILoginState {
  email: string;
  password: string;
  error: string;
}

export default class Login extends React.Component<LoginProps, ILoginState> {
  constructor(props: LoginProps) {
    super(props);
    this.state = {
      email: "",
      password: "",
      error: "",
    };
  }

  // handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
  //   e.preventDefault();

  //   try {
  //     await ApiSingleton.authService.login(
  //       this.state.email,
  //       this.state.password
  //     );
  //     this.props.onLogin?.();
  //   } catch (err) {
  //     if (typeof err === "string") {
  //       this.setState({ error: err });
  //     } else {
  //       console.log(err);
  //       //throw new Error("Unhandled eror." + JSON.stringify(err));
  //     }
  //   }
  // };

  handleSubmit = () => {
    fetch("http://localhost:3001/login")
    .then(res => res.json())
    .then(result => {
      console.log(result);
      if (result.email === this.state.email && result.password === this.state.password){
        this.props.onLogin?.();
      }
    })
  }

  render() {
    const headerStyles: React.CSSProperties = { marginRight: "9.5rem" };

    if (this.state.error) {
      headerStyles.marginBottom = "-1.5rem";
    }

    //if (ApiSingleton.authService.isLoggedIn()) {
    //  this.props.onLogin?.()
    //}

    return (
      <div className="page">
        <Typography component="h1" variant="h5">
          Войти
        </Typography>
        {this.state.error && (
          <p style={{ color: "red", marginBottom: "0" }}>{this.state.error}</p>
        )}
        <form onSubmit={(e) => this.handleSubmit()} className="loginForm">
          <TextField
            required
            type="email"
            size="small"
            label="Email Address"
            variant="outlined"
            margin="normal"
            name={this.state.email}
            onChange={(e) => this.setState({ email: e.target.value })}
          />
          <TextField
            required
            type="password"
            size="small"
            label="Password"
            variant="outlined"
            margin="normal"
            value={this.state.password}
            onChange={(e) => this.setState({ password: e.target.value })}
          />
          <br />
          <Button
            size="small"
            variant="contained"
            color="primary"
            type="submit"
          >
            Войти
          </Button>
        </form>
      </div>
    );
  }
}
