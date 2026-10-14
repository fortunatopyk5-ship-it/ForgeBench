# CMP122-II (1.91): adversarial upstream composition audit

Current verdict (2026-10-14): **GAP — the claimed uniform upstream and composed bounded C^1 theorems are withdrawn.** The abstract conditional implications remain valid. The accepted native downstream theorem in rprime_c1.md is unchanged.

This replaces Iteration-10 claims U24, U29, U47--U52, U56 and the former "revalidation". No assertion here disproves the physical Balaban construction.

## 2026-10-14 continuation: V=0 attack

**The old unrestricted independent operator chart is INVALID as an unqualified uniform theorem.** With mu_F>=kappa, the rank-one family
T_{X_n}=c A_T e^{-kappa d(X_n)}E_n, E_n1=1,
on nested long connected cube chains has ||T||_old<=1/2 and an infinite downstream necessary sum. Every positive scalar radius still contains a rescaled counterexample.

This conclusion has a precise scope: for the exact chart index family P_T the V=0 criterion is
C_Tgeo(lambda_in-beta kappa)<infinity.
The test passes under an additional verified reserve, or direct anchored embedding when mu_F<=mu_T. The actual native restricted-family certificate is UNKNOWN; the all-connected counterexample is not asserted to be a physical history.

The strengthened analysis is [rprime_191_vzero_attack.md](rprime_191_vzero_attack.md):
- full hidden-variable conditional operator/TV norm;
- stronger independent ordinary-decay chart (physical embedding UNKNOWN);
- a genuine open relative-kernel Banach chart with uniform positive radius (local theorem PROVED);
- labelled positive majorant with all kernel/index data retained;
- the q=1,D={Y} relative-V/attached-domain sum remains UNCONTROLLED.

Reduced upstream C^1 and strict whole-ball image inclusion remain GAP. Uniform local kernel radius does not prove uniform output over Sigma. The V-producer is DEFERRED until the reduced map is closed. The accepted downstream theorem is unchanged.

## 1. Exact map and frozen data

For a fixed admissible history/domain fiber Sigma, the recorded primary (1.91), printed p.388, is

F(X') =
 sum_{q>=0} sum'_{ {X_{j_1},...,X_{j_q}} } sum'_D
 [product_{Y in D} integral_0^1 dt(Y)]
 [product_{h=1}^q T'_k(X_{j_h})]
 [product_{Y in D} V(Y)]
 exp[sum_{Y in D} t(Y)V(Y)].

The T' product acts on the expression to its right. D is a subfamily of the Mayer localization family. The primes select exactly the data determining X' by the special component rule preceding (1.90). All integration intervals, determining labels, hard characteristic choices, component rules and localization sets are fixed. In the regular lane disjoint from union_i Y_i, q>=1. An empty D is allowed in the source case X'=X_j (p.389).

Only T' and V occur explicitly. E/R/B and other local coefficients enter through the preceding V producer, or through formation of T' if T' is not already a coordinate. No hard characteristic is differentiated.

Caution on types: (1.72), p.379, states that the history sum inside T' also acts on the V exponential. A local T' operator must therefore act on a space retaining the integration variables AND the restricted history labels on which V depends. Identifying this domain with only the downstream field algebra A_k, without a bounded evaluation/restriction dictionary, has not been justified. The frozen outer fiber does not automatically erase inner history sums.

## 2. Elementary maps and what survives

For I=(q,Xvec,D) in the fixed index fiber put
W_D=sum_D t(Y)V_Y, G_D=(product_D V_Y)exp(W_D),
Phi_I=integral_[0,1]^D (product_h T'_{X_h})G_D.

With explicitly compatible coefficient algebras/operator domains:
- coordinate restriction and finite sums: bounded linear;
- finite V products and ordered T' products: continuous multilinear;
- exponential: entire on a unital coefficient Banach algebra;
- t integration: bounded linear, norm <=1;
- finite Phi_I sums: analytic;
- the full history/domain resummation in the anchored output norm: CONDITIONAL on a uniform absolute convergence bound.

Formal first derivatives of finite terms and constituent support eta subset X' remain valid. For the infinite map the derivative/support theorem is conditional on its existence and normal convergence; it cannot independently prove those facts.

Normalization of a T integral is analytic where its denominator is nonzero. Normalization is not part of (1.91) when unnormalized T' is the independent coordinate.

## 3. Audit of the proposed coefficient ball

The old reduced mixed norm used
M_T(X)=A_T(gamma) exp[-kappa d_k(X)],
M_V(Y)=alpha_gamma exp[-(1+2 beta)kappa d_k(Y)]
as the OLD claimed ordinary-metric V regulator, plus normalized anchored terms. Its source embedding was not established: (1.68) uses a relative region Z_loc, and regular disjointness from Ycal alone does not make every V metric ordinary.

The pointwise regulator gives ||T_X||<=R M_T(X), ||V_Y||<=R M_V(Y) on a radius-R ball. It does NOT prove that every intermediate inequality in the physical source derivation holds there.

Even the normalizers C_T,sum, C_V,sum and C_s in the old chart require their residual exponents to be in the geometric summability regime, not merely positive. Multiplicity in the scale/history/atom labels must also be counted. The old phrase "choose mu_s strictly below the source exponent" is insufficient for this.

A genuine complex-ball majorant, assuming common compatible submultiplicative domains, is

M_{F,Sigma}^{abs}(X';R_T,R_V) =
 sum_{I in I_Sigma^reg(X')}
 [product_h R_T M_T(X_h)]
 [product_{Y in D} R_V M_V(Y)]
 exp[sum_{Y in D} R_V M_V(Y)].

The t integrals have norm <=1, so this follows directly by operator and algebra norms. If restriction/pullback norms are not <=1, their actual factors must be included. This is an extended nonnegative series; its finiteness and anchored summability have NOT been proved. In particular it must not be identified with source (1.97).

The precise reduced missing certificate is

Q(R_T,R_V) :=
 sup_{volume,cutoff,k,Sigma} sup_B
 sum_{X' contains B}
 exp[mu_in d_k(X')] M_{F,Sigma}^{abs}(X';R_T,R_V)
 < r_*^native,

for radii R_T,R_V>1, together with Banach-valued normal convergence on smaller balls. This is YM-191-COMPLEX-MAJORANT-1, currently OPEN.

## 4. Which source estimates can be inflated?

Primary CMP122-II pp.376--380, 387--390 were consulted via the article PDF linked in references.md.

| Source step | What is established | Complex coefficient-ball issue |
| --- | --- | --- |
| (1.68), decaying V class | Bound for the produced V, with alpha small | If imposed as a coordinate regulator, complex modulus bounds inflate to R_V alpha. This alone does not control the producer. |
| (1.73)--(1.75) | Positive real integral density with a controlled small complex exponent sigma; nonzero normalization and normalized operator estimate | Positivity/nonvanishing are structural facts of that integral. Arbitrary independent bounded T' operators need not satisfy them. |
| (1.79)--(1.89) and following tree factor | Large-field/history energy suppression for source T' | In operator coordinates only explicitly proved norm envelopes may be imposed. A scalar T'1 estimate alone does not bound an arbitrary operator. |
| (1.92) | Uses (1.68), (1.73), (1.89), and a relative metric with Z'=union_i Y_i union_h X_h | The old mixed norm does not supply a dictionary reproducing this estimate for arbitrary T'/V operators on hidden history/integration variables. |
| (1.93)--(1.96) | Absorbs component joining, size costs and entropy using small alpha and p_0(g_k) | Numerical modulus-only inequalities admit strict slack, but only after their exact perturbation factors and support hypotheses have been retained. |
| (1.97) | Source-generated regular F has the stated decay | Does not itself extend to an ambient open coefficient ball. |

For example, the recorded source sufficient shapes
alpha^(1/3)<=exp[-(1+beta)kappa 2^d],
O(1)alpha^(1/3)<=(6*2^d)^(-1)beta kappa,
exp[-p_0(g_k)/2]<=(6*2^d)^(-1)beta kappa
can tolerate fixed inflation when all other hypotheses are preserved.
Inflating alpha replaces alpha^(1/3) by R_V^(1/3)alpha^(1/3).
Inflating an operator envelope introduces q log R_T in a q-factor term.
The p.389 absorption also contains 2 kappa(100 R_k)^d; R_k here is a SOURCE geometric scale, not a coefficient-ball radius. Its coupling dependence must remain in the budget.

These examples are not an exhaustive rederivation of (1.92)--(1.97), hence do not establish R_src>1. Complex absolute-value estimates do not require positive coefficients once suitable operator bounds are established; it would also be incorrect to claim that positivity prevents every possible ambient extension.

A useful diagnostic: at V=0, the admissible q=1,D=empty term gives
F(X)=T'_X 1.
Thus any asserted full-ball F envelope must bound every allowed T'_X1. A regulator with only exp[-kappa d(X)] does not, by itself, imply a faster uniform exp[-(1+beta)kappa d(X)] envelope on unbounded supports. Source geometric restrictions or a stronger T' regulator could repair this; neither has been proved for the old ambient chart. This is a necessary check, not a counterexample to Balaban's physical family.

## 5. V producer: conditional row theorem versus application

Already-renormalized E_ren/R_ren/B_loc remains the intended input chart. Reopening E_raw relevant extraction is not the present remedy.

A frozen linear producer would have to be specified as

(P_VK)(Y)=sum_lambda L_{Sigma,Y,lambda} K_lambda,

where L includes restriction, pullback and localization maps between actual coefficient domains. Scalar incidence numbers are insufficient unless those maps are proved contractive. Any fixed add/subtract action atoms must be included in K_aux or treated as an explicit affine offset.

The needed estimate is

sup_{volume,cutoff,k,Sigma,Y}
 M_V(Y)^(-1)
 sum_lambda ||L_{Sigma,Y,lambda}|| M_lambda
 <= C_res < infinity.

The mixed-norm implication from this row estimate is valid:
||P_VK(Y)||<=C_res ||K||_up M_V(Y),
and the anchored output bound follows by summing M_V in a verified residual regime. This requires no two-anchor Schur theorem.

What is missing is its SOURCE INSTANTIATION for every independent coefficient in the advertised product chart. The pp.376--377 proof tracks decay after localizations and renormalizations and refers to earlier resummations; it does not provide in our notebook an explicit L dictionary and uniform row estimate for those exact independently variable atoms. The existence of a physical output bound, even obtained by absolute estimates on intermediate terms, does not substitute for that dictionary. Linearity alone also gives no uniform norm constant.

Furthermore replacing M_V by C_res M_V changes the amplitude entering the reduced nonlinear map to C_res alpha_gamma. Calling ||P_V||<=1 by normalization cannot conceal that change from the complex majorant and image-radius proof.

YM-V-PRODUCER-BOUND-1: source application GAP.
YM-V-PRODUCER-MIXED-NORM-1: abstract implication PROVED UNDER ITS ROW HYPOTHESIS.

## 6. Source output norm: a result that does survive

For the source-generated regular activity, (1.97) gives

M_F^src(X') =
 exp[-p_0(g_k)] exp[-(1+beta)kappa d_k(X')].

Regular disjointness reduces the relative metric to d_k. Use EXACTLY the accepted downstream exponent

mu_F=mu_in=(1+beta)kappa-lambda_in,

with C_geo(lambda_in)<infinity and 16a+theta<=mu_in. It is not permissible to choose a different lambda_F without proving an embedding into the downstream input space.

Then directly

sup_B sum_{X' contains B} exp[mu_in d_k(X')] M_F^src(X')
 <= C_geo(lambda_in) exp[-p_0(g_k)]
 <= C_geo(lambda_in) exp[-p_0(gamma)].

This bound uses the already accepted source/geometric hypotheses uniformly over admissible sectors; restricting their domain families only decreases the common geometric sum. Consequently the PHYSICAL SOURCE FAMILY can be placed strictly inside

r_*^native=(a/9)exp[-16a-sqrt(7)theta]

by a fixed small-coupling threshold. This is not the supremum over a complex coefficient ball.

The source summability certificate is PROVED under the accepted source assumptions. The inflated-ball analogue and full upstream uniformity in Sigma are UNKNOWN.

## 7. Correct radius and chain-rule certificate (conditional only)

Suppose the missing producer estimate is proved with constant C_P and with any affine offset accounted for. Suppose the reduced map is holomorphic on a radius-R_red ball with uniform output bound Q_red<r_*^native.

To obtain a ball containing the upstream unit ball one must prove, in the original physical normalization, C_P R_up<R_red for some R_up>1 (or the corresponding affine bound). Normalizing C_P to one does not prove this inequality.

More generally it is enough to certify directly

sup_{||K||_up<R_up} ||U_Sigma(K)||_{B_F(mu_in)}
 <= r_image < r_*^native,  R_up>1,

uniformly in volume, cutoff, k and Sigma, with U holomorphic and bounded there. This is YM-UPSTREAM-IMAGE-RESERVE-1, OPEN.

Only after that certificate, set
rho=(r_image+r_*^native)/2,
q_native(rho)=exp[16a+sqrt(7)theta]rho.

On ||K||_up<=1, Banach Cauchy would give
||DU_Sigma||<=r_image/(R_up-1).
The accepted downstream theorem would give
||DH_R'(U_Sigma(K))||<=q_native(rho)/(rho-r_image).

Hence the valid conditional chain rule is

D(H_R' o U_Sigma)_K =
 DH_R'_{U_Sigma(K)} o DU_{Sigma,K},

with uniform bound
q_native(rho)/(rho-r_image) * r_image/(R_up-1).

Domains are B_{R_up}(B_up^(Sigma)) -> B_{r_image}(B_F(mu_in)) -> B_R'(theta), with the first output bound interpreted as <=r_image and downstream holomorphy on B_{r_*}(B_F(mu_in)). A value at one physical input cannot justify the coefficient derivative.

The prerequisites are not proved, so this chain rule is NOT applied as a theorem of the source-native family.

## 8. Final verdict and exact blocker

- UPSTREAM C^1: GAP (conditional theorem available).
- COMPLEX BALL: GAP.
- OUTPUT IMAGE STRICTLY INSIDE R' BALL: NO — not established for the requested whole ball; YES only for the source-generated subfamily under its hypotheses.
- COMPOSED C^1: GAP (unconditional claim withdrawn).
- UNIFORMITY IN Sigma: UNKNOWN for the independent coefficient-ball map.
- EXACT NEW BLOCKER: YM-191-COMPLEX-MAJORANT-1 + the source instantiation of the V row/operator dictionary, culminating in YM-UPSTREAM-IMAGE-RESERVE-1.

The first failed inference is "physical absolute source bounds + strict numerical slack => all independent complex coefficient families satisfy the same anchored majorant". This occurs already in the REDUCED (T',V) theorem, before invoking the producer or chain rule. A marked/off-diagonal Schur estimate is not required to repair bounded C^1; an unmarked whole-ball majorant with the exact downstream exponent IS required.

No downstream re-proof, K_0, STEP_0, boundary B' or other RG-sector work was undertaken.
