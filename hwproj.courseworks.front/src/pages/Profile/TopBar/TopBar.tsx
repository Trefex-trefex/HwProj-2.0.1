import React, { Component } from "react";
import { Logotype, Toast, TopBar } from "@skbkontur/react-ui";
import { User } from "@skbkontur/react-icons";
import styles from "./TopBar.module.css";
import { IUser } from "types";

interface Props {
  user: IUser;
  logout(): void;
}

interface State {
  isAccountTitleHovered: boolean;
}

export default class ServiceTopBar extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { isAccountTitleHovered: false };
  }

  render() {
    return this.renderTopBar();
  }

  private renderTopBar = () => {
    const { isAccountTitleHovered } = this.state;
    return (
      <TopBar>
        <TopBar.Start>
          <TopBar.ItemStatic>
            <div
              style={{
                display: "flex",
                fontSize: 25,
                position: "absolute",
                left: "1rem",
              }}
            >
              <span style={{ fontSize: 12.5, marginRight: "1rem" }}>
                {this.props.user.role.toUpperCase()}
              </span>
              <Logotype
                suffix="курсач"
                locale={{ prefix: "hwpr", suffix: "j" }}
                color="#1E79BE"
              />
            </div>
          </TopBar.ItemStatic>
        </TopBar.Start>
        <TopBar.End>
          <div
            onMouseEnter={() => this.setState({ isAccountTitleHovered: true })}
            onMouseLeave={() => this.setState({ isAccountTitleHovered: false })}
            className={styles.accountTitle}
          >
            <TopBar.ItemStatic
              active={isAccountTitleHovered}
              _onClick={this.onAccountTitleClick}
            >
              <User color="#666" />
              &nbsp;{this.props.user.firstName} {this.props.user.lastName}
            </TopBar.ItemStatic>
          </div>
          <TopBar.Divider />
          <TopBar.Logout
            onClick={() => {
              //-----------------------
              // Запрос на выход
              //-----------------------
              Toast.push("Logout");
              this.props.logout();
            }}
          />
        </TopBar.End>
      </TopBar>
    );
  };

  private onAccountTitleClick = () => {
    Toast.push("Этот раздел ещё в разработке", {
      label: "Окей, я понял",
      handler: () => Toast.push("Допиши сам!"),
    });
  };
}
